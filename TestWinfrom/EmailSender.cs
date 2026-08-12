using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;

namespace TestWinfrom
{
    public class EmailSender
    {
        #region 邮箱配置，正式环境建议移到配置文件
        private readonly string _smtpHost = "smtp.qq.com";
        private readonly int _smtpPort = 465;       // 适配SslOnConnect
        private readonly string _senderName = "系统通知";
        private readonly string _senderEmail = "625705479@qq.com";
        private readonly string _authCode = "wchawgzqkzqwbbaj";
        #endregion

        /// <summary>
        /// 获取模板目录绝对路径
        /// </summary>
        public static string GetTemplatePath(string templateName)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, templateName);
        }

        /// <summary>
        /// 发送VM模板HTML邮件【异步】
        /// </summary>
        /// <param name="toEmail">收件人邮箱，多个用英文逗号分隔</param>
        /// <param name="emailSubject">邮件标题</param>
        /// <param name="templateFileName">模板文件名：NotifyEmail.vm</param>
        /// <param name="model">模板参数</param>
        public async Task SendTemplateMailAsync(string toEmail, string emailSubject, string templateFileName, Dictionary<string, object> model)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("收件人邮箱不能为空");

            try
            {
                string tplPath = GetTemplatePath(templateFileName);
                string htmlBody = VelocityHelper.Render(tplPath, model);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_senderName, _senderEmail));

                // 处理多个收件人：英文逗号分割，清洗空格、空地址
                var emailList = toEmail.Split(',')
                    .Select(e => e.Trim())
                    .Where(e => !string.IsNullOrEmpty(e));

                foreach (var email in emailList)
                {
                    message.To.Add(new MailboxAddress(string.Empty, email));
                }

                message.Subject = emailSubject; // 【重要修复】你原来代码缺失标题！
                message.Body = new TextPart("html")
                {
                    Text = htmlBody
                };

                // using 自动释放连接
                using (var client = new SmtpClient())
                {
                    // 解决国内证书吊销检查失败报错
                    client.CheckCertificateRevocation = false;
                    client.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                    {
                        // 仅放行吊销检查失败，其他证书错误拦截；如果需要彻底调试可直接 return true
                        if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNotAvailable)
                            return true;
                        return sslPolicyErrors == SslPolicyErrors.None;
                    };

                    await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.SslOnConnect);
                    await client.AuthenticateAsync(_senderEmail, _authCode);
                    await client.SendAsync(message, CancellationToken.None);
                    await client.DisconnectAsync(true, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"邮件发送异常：{ex.Message}\r\n{ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// 同步版本（Winform按钮点击调用）
        /// </summary>
        public async Task SendTemplateMail(string toEmail, string emailSubject, string templateFileName, Dictionary<string, object> model)
        {
            // .ConfigureAwait(false).Wait() 减少WinForm上下文死锁风险
            await SendTemplateMailAsync(toEmail, emailSubject, templateFileName, model).ConfigureAwait(false);
        }
    }
}