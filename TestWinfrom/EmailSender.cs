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
        private readonly SmtpConfig _smtpConfig;

        public EmailSender(SmtpConfig smtpConfig)
        {
            _smtpConfig = smtpConfig ?? throw new ArgumentNullException(nameof(smtpConfig));
        }

        public static string GetTemplatePath(string templateName)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, templateName);
        }

        /// <summary>
        /// 发送模板邮件【异步主方法，支持附件】
        /// </summary>
        /// <param name="toEmail">收件人，多个邮箱逗号分隔</param>
        /// <param name="emailSubject">邮件标题</param>
        /// <param name="templateFileName">Velocity模板文件名</param>
        /// <param name="model">模板渲染数据</param>
        /// <param name="attachmentFiles">附件路径集合，不传=null无附件</param>
        public async Task SendTemplateMailAsync(string toEmail,
            string emailSubject,
            string templateFileName,
            Dictionary<string, object> model,
            List<string> attachmentFiles = null)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("收件邮箱不能为空", nameof(toEmail));
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            string tplPath = GetTemplatePath(templateFileName);
            if (!File.Exists(tplPath))
                throw new FileNotFoundException("邮件模板文件不存在", tplPath);

            string htmlBody = VelocityHelper.Render(tplPath, model);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpConfig.SenderName, _smtpConfig.SenderEmail));

            var emailList = toEmail.Split(',')
                .Select(e => e.Trim())
                .Where(e => !string.IsNullOrEmpty(e));
            foreach (var email in emailList)
            {
                message.To.Add(new MailboxAddress("", email));
            }

            message.Subject = emailSubject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = htmlBody;

            // 加载附件
            if (attachmentFiles != null && attachmentFiles.Any())
            {
                foreach (var filePath in attachmentFiles)
                {
                    if (File.Exists(filePath))
                    {
                        bodyBuilder.Attachments.Add(filePath);
                    }
                }
            }

            message.Body = bodyBuilder.ToMessageBody();

            
             var client = new SmtpClient();
            client.CheckCertificateRevocation = false;
            client.ServerCertificateValidationCallback = (s, cert, chain, err) => true;

            await client.ConnectAsync(_smtpConfig.Host, _smtpConfig.Port, _smtpConfig.SecureOption);
            await client.AuthenticateAsync(_smtpConfig.SenderEmail, _smtpConfig.Password);
            await client.SendAsync(message, CancellationToken.None);
            await client.DisconnectAsync(true, CancellationToken.None);
        }

        /// <summary>
        /// 兼容旧调用：不带附件版本
        /// </summary>
        public async Task SendTemplateMailAsync(string toEmail, string emailSubject, string templateFileName, Dictionary<string, object> model)
        {
            await SendTemplateMailAsync(toEmail, emailSubject, templateFileName, model, null);
        }

     
    }
}