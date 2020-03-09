using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Aspose.Email;
using Aspose.Email.Clients;
using Aspose.Email.Clients.Smtp;
using Microsoft.Extensions.Logging;
using Supertext.Base.Exceptions;

namespace Supertext.Base.Net.Mail
{
    public class MailService
    {
        private static ILogger<MailService> _logger;
        private readonly MailServiceConfig _configuration;

        public MailService(ILogger<MailService> logger, MailServiceConfig configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public void Send(EmailInfo mail)
        {
            try
            {
                MailMessage HandleHtml(MailMessage message)
                {
                    message.IsBodyHtml = true;
                    message.Body = mail.Message;
                    return message;
                }

                SendInternal(mail,
                             HandleHtml);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Send)}: Couldn't send email. To={mail.To.Email}; Subject={mail.Subject}", ex);
                throw;
            }
        }

        public void SendAsHtml(EmailInfo mail)
        {
            try
            {
                MailMessage HandleHtml(MailMessage message)
                {
                    message.IsBodyHtml = true;
                    message.HtmlBody = mail.Message;
                    return message;
                }

                SendInternal(mail,HandleHtml);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(SendAsHtml)}: Couldn't send email. To={mail.To.Email}; Subject={mail.Subject}", ex);
                throw;
            }
        }

        private void SendInternal(EmailInfo mail, Func<MailMessage, MailMessage> handleHtml)
        {
            var msg = new MailMessage();

            msg = CreateEmail(mail, msg, handleHtml);

            var client = new SmtpClient();

            if (_configuration.SendGridEnabled)
            {
                client = SetSendGridClient(client);
            }
            else
            {
                client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                client.PickupDirectoryLocation = _configuration.LocalEmailDirectory;
            }

            var attachFileStreamsWithNames = mail.Attachments.Select(att => new Tuple<Stream, string>(ConvertToFileStream(att.Content), att.Name)).ToList();
            try
            {
                attachFileStreamsWithNames.ForEach(attachment => msg.AddAttachment(new Attachment(attachment.Item1, attachment.Item2)));
                client.Send(msg);
            }
            finally
            {
                // dispose of each of the attachment streams
                foreach (var attachFileStreamWithName in attachFileStreamsWithNames)
                {
                    attachFileStreamWithName.Item1.Dispose();
                }

                // dispose of each attachment
                foreach (var attachment in msg.Attachments)
                {
                    attachment.Dispose();
                }

                client.Dispose();
                msg.Dispose();
            }

            _logger.LogInformation($"Email sent. To={mail.To.Email}; Subject={mail.Subject}");
        }

        private MailMessage CreateEmail(EmailInfo mail, MailMessage msg, Func<MailMessage, MailMessage> handleHtml)
        {
            msg = handleHtml(msg);
            msg.BodyEncoding = System.Text.Encoding.UTF8;
            msg.From = new MailAddress(mail.From.Email, mail.From.Name);
            msg.Subject = mail.Subject;
            msg.SubjectEncoding = System.Text.Encoding.UTF8;


            msg.To.Add(new MailAddress(mail.To.Email, mail.To.Name));

            if (!String.IsNullOrEmpty(mail.BccEmail))
            {
                msg.Bcc.Add(new MailAddress(mail.BccEmail));
            }

            if (!String.IsNullOrEmpty(mail.ReplyTo))
            {
                msg.ReplyToList = new MailAddress(mail.ReplyTo, mail.ReplyToName);
            }

            return msg;
        }

        private SmtpClient SetSendGridClient(SmtpClient client)
        {
            var port = _configuration.SendGridPort;

            client.SecurityOptions = SecurityOptions.SSLExplicit;
            client.UseAuthentication = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Host = _configuration.SendGridHost;
            client.Password = _configuration.SendGridPassword;
            client.Port = port;
            client.Username = _configuration.SendGridUsername;

            if (String.IsNullOrWhiteSpace(client.Host))
            {
                Console.WriteLine("The SendGrid 'host' property was not set in the configuration's AppSettings.");
                throw new ConfigurationException("The SendGrid 'host' property was not set in the configuration's AppSettings.");
            }

            if (String.IsNullOrWhiteSpace(client.Password))
            {
                Console.WriteLine("The SendGrid 'password' property was not set in the configuration's AppSettings.");
                throw new ConfigurationException("The SendGrid 'password' property was not set in the configuration's AppSettings.");
            }

            if (String.IsNullOrWhiteSpace(client.Username))
            {
                Console.WriteLine("The SendGrid 'username' property was not set in the configuration's AppSettings.");
                throw new ConfigurationException("The SendGrid 'username' property was not set in the configuration's AppSettings.");
            }

            return client;
        }

        private static FileStream ConvertToFileStream(byte[] content)
        {
            var memStream = new MemoryStream();
            var binForm = new BinaryFormatter();
            memStream.Write(content, 0, content.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            return (FileStream)binForm.Deserialize(memStream);
        }
    }
}