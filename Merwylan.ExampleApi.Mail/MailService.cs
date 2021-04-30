using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Hangfire;
using Merwylan.ExampleApi.Shared.Extensions;
using Microsoft.Extensions.Logging;

namespace Merwylan.ExampleApi.Mail
{
    public class MailService : IMailService
    {
        private readonly SmtpClient _client;
        private readonly string _sender;
        private readonly string _recipients;
        private readonly ILogger<IMailService> _logger;

        public MailService(ILogger<IMailService> logger, MailConfig config)
        {
            _logger = logger;
            _client = GetSmtpClient(config);
            _sender = config.Sender;
            _recipients = config.Recipients;
        }

        public void FireAndForgetMail(MailModel model)
        {
            BackgroundJob.Enqueue(() => SendMail(model));
        }

        public void SendMail(MailModel model)
        {
            var recipients = _recipients.Split(";").ToList();
            recipients.AddRange(model.ExtraRecipients);
            var recipientsString = string.Join(',', recipients);

            var email = new MailMessage(_sender, recipientsString, model.Subject, model.Message);

            foreach (var attachment in model.AttachmentFilePaths)
            {
                if (string.IsNullOrEmpty(attachment) || !email.Attachments.TryAdd(new Attachment(attachment)))
                {
                    _logger.LogWarning($"Failed to add attachment {attachment} to mail: {model.SerializeCamelCase()}");
                }
            }
            
            try
            {
                _client.Send(email);
                _logger.LogInformation($"Successfully sent mail: {model.SerializeCamelCase()}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occurred while trying to send a mail: {model.SerializeCamelCase()}");
            }
        }
            

        private SmtpClient GetSmtpClient(MailConfig config)
        {
            return new SmtpClient(config.SmtpServer)
            {
                Port = config.Port,
                Credentials = new NetworkCredential(config.Sender, config.SenderPassword),
                EnableSsl = true,
            };
        }
    }
}
