using System.ComponentModel;
using System.Reflection;
using System.Text;
using WebApiProject1.Application.Test.Dtos;

namespace WebApiProject1.Application.UntinesHelper
{
    public static class Untines
    {
        private static readonly char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        private static readonly Random random = new Random();
        public static Type GetNonNullableType(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return type.GetGenericArguments()[0];
            }

            // 如果类型不是 Nullable<>，则直接返回原类型  
            return type;
        }
        internal static (string, byte[]) Create()
        {
            string code = GenerateCode(4);
            byte[] imageBytes = GenerateImage(code);
            return (code, imageBytes);
        }
        /// <summary>
        /// 获取随机验证码
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private static string GenerateCode(int length)
        {
            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[random.Next(chars.Length)]);
            }
            return sb.ToString();
        }

        private static byte[] GenerateImage(string code)
        {
            return new byte[] { };
        }
        /// <summary>
        /// 封装错误信息设置方法
        /// </summary>
        /// <param name="result"></param>
        /// <param name="myError"></param>
        /// <param name="Message"></param>
        public static void SetError(ResultData<object> result, EnumExtensions.MyErrorEnum myError, string Message = null)
        {
            BaseResponse baseResponse = new BaseResponse();

            baseResponse.StatusCode = 500;
            baseResponse.ChineseError = myError.GetChinese();
            baseResponse.EnglishError = myError.GetEnglish();
            baseResponse.Message = Message;
            result.BaseResponse = baseResponse;
            Logger.Error("接口异常:" + baseResponse.StatusCode + baseResponse.Message + baseResponse.ChineseError + baseResponse.EnglishError);

        }


// <summary>
/// 保存数据到文件缓存中
/// </summary>
/// <param name="key">缓存键</param>
/// <param name="value">缓存值</param>
/// <param name="timeSpan">缓存有效期</param>
public static void SaveToFileCache(string key, string value, TimeSpan timeSpan)
    {
        var cacheDir = Path.Combine(AppContext.BaseDirectory, "Cache");
        Directory.CreateDirectory(cacheDir);

        var filePath = Path.Combine(cacheDir, key);
        var content = $"{value}|{DateTime.Now.Add(timeSpan):o}"; // 包含过期时间

        File.WriteAllText(filePath, content);
    }

    /// <summary>从文件缓存读取，过期返回null</summary>
    public static string? ReadFromFileCache(string key)
    {
        var cacheDir = Path.Combine(AppContext.BaseDirectory, "Cache");
        var filePath = Path.Combine(cacheDir, key);
        if (!File.Exists(filePath)) return null;

        var text = File.ReadAllText(filePath);
        var parts = text.Split(new[] { '|' }, 2);
        if (parts.Length != 2)
        {
            File.Delete(filePath);
            return null;
        }

        var value = parts[0];
        if (!DateTime.TryParse(parts[1], out var expireTime))
        {
            File.Delete(filePath);
            return null;
        }

        // 判断是否过期
        if (DateTime.Now > expireTime)
        {
            File.Delete(filePath);
            return null;
        }
        return value;
    }

        private static readonly TimeSpan _scanPeriod = TimeSpan.FromMinutes(3);

        public static  async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var cacheDir = Path.Combine(AppContext.BaseDirectory, "Cache");
                    if (Directory.Exists(cacheDir))
                    {
                        var files = Directory.GetFiles(cacheDir);
                        foreach (var filePath in files)
                        {
                            try
                            {
                                string content = await File.ReadAllTextAsync(filePath, stoppingToken);
                                int splitIndex = content.LastIndexOf('|');
                                if (splitIndex <= 0)
                                {
                                    File.Delete(filePath);
                                    continue;
                                }

                                string expireText = content.Substring(splitIndex + 1);
                                if (DateTime.TryParse(expireText, out DateTime expireTime))
                                {
                                    if (DateTime.Now > expireTime)
                                    {
                                        // 已经过期，物理删除磁盘文件
                                        File.Delete(filePath);
                                    }
                                }
                                else
                                {
                                    // 时间格式损坏，直接删除垃圾文件
                                    File.Delete(filePath);
                                }
                            }
                            catch (IOException)
                            {
                                // 文件被别的进程占用，跳过本次，下一轮扫描再处理
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // 扫描异常不崩溃主程序，吞掉异常
                }

                await Task.Delay(_scanPeriod, stoppingToken);
            }
        }
        /// <summary>删除文件缓存</summary>
        public static void RemoveFileCache(string key)
    {
        var cacheDir = Path.Combine(AppContext.BaseDirectory, "Cache");
        var filePath = Path.Combine(cacheDir, key);
        if (File.Exists(filePath))
            File.Delete(filePath);
    }

        public static T GetEnumByDescription<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var desc = field.GetCustomAttribute<DescriptionAttribute>();
                if (desc != null && desc.Description == description)
                {
                    return (T)field.GetValue(null);
                }
            }
            return default;
        }
        /// <summary>获取枚举的Description中文描述</summary>
        /// <summary>获取LeaveFlowStatus枚举的中文Description描述</summary>
        public static string GetDescription(this Enum enumValue)
        {
            FieldInfo field = enumValue.GetType().GetField(enumValue.ToString());
            DescriptionAttribute attr = field.GetCustomAttribute<DescriptionAttribute>();
            return attr != null ? attr.Description : enumValue.ToString();
        }

        /// <summary>
        /// 获取中文
        /// </summary>
        public static string GetZh(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return string.Empty;
            var arr = source.Split('/');
            return arr.Length >= 1 ? arr[0].Trim() : source;
        }

        /// <summary>
        /// 获取英文
        /// </summary>
        public static string GetEn(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return string.Empty;
            var arr = source.Split('/');
            return arr.Length >= 2 ? arr[1].Trim() : string.Empty;
        }

        /// <summary>
        /// 自动切换语言
        /// </summary>
        public static string GetMsg(string source, bool isZh = true)
        {
            return isZh ? GetZh(source) : GetEn(source);
        }
    }
}
