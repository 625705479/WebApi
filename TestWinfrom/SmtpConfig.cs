using MailKit.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWinfrom
{
    /// <summary>SMTP通道配置</summary>
    public class SmtpConfig
    {
        public string ChannelKey { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

        // 原始枚举，不参与直接序列化
        public SecureSocketOptions SecureOption { get; set; }

        // JSON读写字符串中转字段
        public string SecureOptionText
        {
            get => SecureOption.ToString();
            set => SecureOption = (SecureSocketOptions)Enum.Parse(typeof(SecureSocketOptions), value);
        }


        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string Password { get; set; }
        public List<string> MatchDomains { get; set; } = new List<string>();
    }

    // 外层包装，对应json根节点
    public class SmtpChannelRoot
    {
        public List<SmtpConfig> Channels { get; set; } = new List<SmtpConfig>();
    }
    public static class ConfigLoader
    {
        /// <summary>加载邮件通道JSON配置</summary>
        public static SmtpChannelRoot LoadEmailChannels(string fileName = "EmailChannels.json")
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            if (!File.Exists(path))
                throw new FileNotFoundException("邮件通道配置文件不存在：" + path);

            string json = File.ReadAllText(path);
            var root = JsonConvert.DeserializeObject<SmtpChannelRoot>(json);
            return root;
        }
    }
}
