using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWinfrom
{
    /// <summary>SMTP通道管理器，实现自动路由切换</summary>
    public class SmtpChannelManager
    {
        private readonly List<SmtpConfig> _channelList;
        private readonly SmtpConfig _defaultChannel;

        public SmtpChannelManager(List<SmtpConfig> channels)
        {
            _channelList = channels ?? throw new ArgumentNullException();
            // 默认通道取第一条，也可以自己指定
            _defaultChannel = _channelList.First();
        }

        /// <summary>根据收件邮箱自动匹配对应的发件通道</summary>
        public SmtpConfig GetAutoChannel(string toEmail)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                return _defaultChannel;

            // 只取第一个收件人域名作为判断依据
            string firstAddr = toEmail.Split(',').First().Trim();
            if (!firstAddr.Contains("@"))
                return _defaultChannel;

            string domain = firstAddr.Split('@')[1].ToLower();

            // 查找配置里匹配该域名的通道
            var matchChannel = _channelList
                .FirstOrDefault(c => c.MatchDomains.Contains(domain));

            return matchChannel ?? _defaultChannel;
        }

        /// <summary>故障转移：发送失败自动切换下一个通道重试</summary>
        public async Task<bool> SendWithFallbackAsync(string toEmail, string subject, string tplFile, Dictionary<string, object> model)
        {
            foreach (var channel in _channelList)
            {
                try
                {
                    var sender = new EmailSender(channel);
                    await sender.SendTemplateMailAsync(toEmail, subject, tplFile, model);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"通道【{channel.ChannelKey}】发送失败：{ex.Message}，尝试下一条通道");
                }
            }
            return false;
        }
    }
}
