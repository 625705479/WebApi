using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TestWinfrom
{
   

    /// <summary>授权主体信息</summary>
    public class LicenseInfo
    {
        /// <summary>绑定机器码</summary>
        public string MachineCode { get; set; }
        /// <summary>授权到期时间</summary>
        public DateTime ExpireTime { get; set; }
        /// <summary>备注信息</summary>
        public string Remark { get; set; }
    }
    /// <summary>运行记录（防时间回拨、备份lic续命）</summary>
    public class RunRecordInfo
    {
        /// <summary>上一次正常启动时间</summary>
        public DateTime LastRunTime { get; set; }
        /// <summary>程序运行记录的最大时间</summary>
        public DateTime MaxRunTime { get; set; }
    }

    public static class RsaSignHelper
    {
        /// <summary>仅管理员工具执行，生成RSA公私钥对</summary>
        public static void GenerateRsaKeyPair(out string privateXml, out string publicXml)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                privateXml = rsa.ToXmlString(true);
                publicXml = rsa.ToXmlString(false);
            }
        }

        /// <summary>私钥签名（仅授权工具）</summary>
        public static string SignData(string rawText, string privateKeyXml)
        {
            byte[] data = Encoding.UTF8.GetBytes(rawText);
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKeyXml);
                byte[] sign = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return Convert.ToBase64String(sign);
            }
        }

        /// <summary>公钥验签（客户端）</summary>
        public static bool VerifyData(string rawText, string signBase64, string publicKeyXml)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(rawText);
                byte[] sign = Convert.FromBase64String(signBase64);
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    rsa.FromXmlString(publicKeyXml);
                    return rsa.VerifyData(data, sign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                }
            }
            catch
            {
                return false;
            }
        }
    }

    public static class SimpleCrypto
    {
        // 自行修改密钥，用于加密 run.rec
        private const string CryptoKey = "RunRecAuthSecret2026999";

        private static byte[] GetFixedKey()
        {
            byte[] src = Encoding.UTF8.GetBytes(CryptoKey);
            byte[] buf = new byte[32];
            int len = Math.Min(src.Length, 32);
            Buffer.BlockCopy(src, 0, buf, 0, len);
            return buf;
        }

        public static string EncryptText(string text)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = GetFixedKey();
                aes.IV = new byte[16];
                ICryptoTransform encryptor = aes.CreateEncryptor();
                byte[] input = Encoding.UTF8.GetBytes(text);
                byte[] output = encryptor.TransformFinalBlock(input, 0, input.Length);
                return Convert.ToBase64String(output);
            }
        }

        public static string DecryptText(string cipher)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = GetFixedKey();
                    aes.IV = new byte[16];
                    ICryptoTransform decryptor = aes.CreateDecryptor();
                    byte[] input = Convert.FromBase64String(cipher);
                    byte[] output = decryptor.TransformFinalBlock(input, 0, input.Length);
                    return Encoding.UTF8.GetString(output);
                }
            }
            catch
            {
                return null;
            }
        }
    }

    public static class HardwareHelper
    {
        private static string GetCpuId()
        {
            try
            {
                using (ManagementClass mc = new ManagementClass("Win32_Processor"))
                {
                    foreach (ManagementObject o in mc.GetInstances())
                    {
                        string val = o["ProcessorId"]?.ToString().Trim();
                        return val ?? "";
                    }
                }
            }
            catch
            {
            }
            return "UnknownCPU";
        }

        private static string GetMotherBoardId()
        {
            try
            {
                using (ManagementClass mc = new ManagementClass("Win32_BaseBoard"))
                {
                    foreach (ManagementObject o in mc.GetInstances())
                    {
                        string val = o["SerialNumber"]?.ToString().Trim();
                        return val ?? "";
                    }
                }
            }
            catch
            {
            }
            return "UnknownMB";
        }

        private static string GetDiskId()
        {
            try
            {
                using (ManagementClass mc = new ManagementClass("Win32_DiskDrive"))
                {
                    foreach (ManagementObject o in mc.GetInstances())
                    {
                        string val = o["SerialNumber"]?.ToString().Trim();
                        if (!string.IsNullOrEmpty(val))
                            return val;
                    }
                }
            }
            catch
            {
            }
            return "UnknownDisk";
        }

        /// <summary>获取本机机器码</summary>
        public static string GetMachineCode()
        {
            string cpu = GetCpuId();
            string mb = GetMotherBoardId();
            string disk = GetDiskId();
            string raw = $"{cpu}|{mb}|{disk}";

            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(raw);
                byte[] hash = md5.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }

    public static class LicenseManager
    {
        // =========【重要】替换为你生成的公钥！=========
        private const string ClientPublicKey = @"<RSAKeyValue><Modulus>4bKHRo2yC/BbYwa+UoDueB81Cywy1dO/rQrLbfzkXP4K2W7GDIO7XMs24mk7SY0osVFZnQA+qFau5TKF2MCsf8fyyOJxblDxi4pqa3nQwM4jCeLdvZL7Qj+YBXKfNjkRJGsJJi2YAkI2M1yOc48q5CWAEftz3Jq/mcghZOoOe3Y4okeuioSy3MrnCadK+cQX3Fkrhklpjf14s6LRErCp2C8gA0On/9X9RO7peCoJ8udCwlxHrpIxrh6eDGLoDwiaLIUYA72QTcYAwMOAk+0Ir6ihnz0b3AYFErVw1/8CaCQfdWs2wYS41zWkZIG7stG0vjGnBwftKje6uwj+fBtTKQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        private static string LicensePath
        {
            get
            {
                return Path.Combine(AppContext.BaseDirectory, "license.lic");
            }
        }

        private static string RunRecordPath
        {
            get
            {
                return Path.Combine(AppContext.BaseDirectory, "run.rec");
            }
        }

        private static RunRecordInfo LoadRunRecord()
        {
            RunRecordInfo record = new RunRecordInfo();
            if (!File.Exists(RunRecordPath))
            {
                record.LastRunTime = DateTime.Now;
                record.MaxRunTime = DateTime.Now;
                return record;
            }

            try
            {
                string cipher = File.ReadAllText(RunRecordPath);
                string json = SimpleCrypto.DecryptText(cipher);
                if (!string.IsNullOrEmpty(json))
                {
                    record = JsonConvert.DeserializeObject<RunRecordInfo>(json);
                }
            }
            catch
            {
                record.LastRunTime = DateTime.Now;
                record.MaxRunTime = DateTime.Now;
            }
            return record;
        }

        private static void SaveRunRecord(RunRecordInfo record)
        {
            try
            {
                string json = JsonConvert.SerializeObject(record);
                string cipher = SimpleCrypto.EncryptText(json);
                File.WriteAllText(RunRecordPath, cipher);
            }
            catch
            {
            }
        }

        #region 授权工具：生成授权码（仅管理员使用）
        public static string GenerateLicenseCode(string machineCode, DateTime expire, string privateKeyXml, string remark = "")
        {
            LicenseInfo info = new LicenseInfo();
            info.MachineCode = machineCode;
            info.ExpireTime = expire;
            info.Remark = remark;

            string json = JsonConvert.SerializeObject(info);
            string sign = RsaSignHelper.SignData(json, privateKeyXml);
            string licenseText = $"{json}|{sign}";

            // 新增：整体Base64封装，对外输出激活码
            byte[] data = Encoding.UTF8.GetBytes(licenseText);
            return Convert.ToBase64String(data);
        }
        #endregion

        #region 客户端：保存授权文件
        public static void SaveLicense(string licenseCode)
        {
            File.WriteAllText(LicensePath, licenseCode);
            // 激活时重置运行记录
            RunRecordInfo rec = new RunRecordInfo();
            rec.LastRunTime = DateTime.Now;
            rec.MaxRunTime = DateTime.Now;
            SaveRunRecord(rec);
        }
        #endregion

        #region 客户端启动校验
        public static bool CheckLicense(out string msg)
        {
            msg = "";
            if (!File.Exists(LicensePath))
            {
                msg = "未检测到授权文件，请激活软件";
                return false;
            }

            string base64Content = File.ReadAllText(LicensePath);
           
                byte[] rawBytes = Convert.FromBase64String(base64Content);
                string content = Encoding.UTF8.GetString(rawBytes);

            

            string[] arr = content.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length != 2)
            {
                msg = "授权文件格式非法";
                return false;
            }

            string jsonText = arr[0];
            string signText = arr[1];

            // RSA验签，防止篡改授权信息
            bool verifyPass = RsaSignHelper.VerifyData(jsonText, signText, ClientPublicKey);
            if (!verifyPass)
            {
                msg = "授权文件已被篡改，验证失败";
                return false;
            }

            LicenseInfo license;
            try
            {
                license = JsonConvert.DeserializeObject<LicenseInfo>(jsonText);
            }
            catch
            {
                msg = "授权信息解析失败";
                return false;
            }

            // 一机一码校验
            string localMachineCode = HardwareHelper.GetMachineCode();
            if (!license.MachineCode.Equals(localMachineCode, StringComparison.OrdinalIgnoreCase))
            {
                msg = "授权与当前设备不匹配";
                return false;
            }

            RunRecordInfo runRec = LoadRunRecord();
            DateTime now = DateTime.Now;

            // 检测时间回拨
            if (now < runRec.LastRunTime)
            {
                msg = "检测到系统时间异常，禁止使用";
                return false;
            }

            // 更新最大运行时间
            if (now > runRec.MaxRunTime)
            {
                runRec.MaxRunTime = now;
            }
            runRec.LastRunTime = now;
            SaveRunRecord(runRec);

            // 授权时效判断
            if (now > license.ExpireTime)
            {
                msg = $"授权已过期，到期时间：{license.ExpireTime:yyyy-MM-dd HH:mm}";
                try
                {
                    if (File.Exists(LicensePath))
                    {
                        File.Delete(LicensePath);
                    }
                }
                catch
                {
                }
                return false;
            }

            // 防御：曾经运行时间超过到期时间，禁止使用（防止备份lic覆盖）
            if (runRec.MaxRunTime > license.ExpireTime)
            {
                msg = "授权有效期已结束";
                try
                {
                    if (File.Exists(LicensePath))
                    {
                        File.Delete(LicensePath);
                    }
                }
                catch
                {
                }
                return false;
            }

            msg = "授权验证通过";
            return true;
        }
        #endregion
    }
}
