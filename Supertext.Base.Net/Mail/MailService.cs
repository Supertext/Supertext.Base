﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspose.Email;
using Aspose.Email.Clients;
using Aspose.Email.Clients.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Supertext.Base.Exceptions;

namespace Supertext.Base.Net.Mail
{
    internal class MailService : IMailService
    {
        private static ILogger<IMailService> _logger;
        private readonly MailServiceConfig _mailServiceConfig;
        private readonly IConfiguration _configuration;

        public MailService(ILogger<IMailService> logger, MailServiceConfig mailServiceConfig, IConfiguration configuration)
        {
            _logger = logger;
            _mailServiceConfig = mailServiceConfig;
            _configuration = configuration;
        }

        public async Task SendAsync(EmailInfo mail)
        {
            try
            {
                await SendInternal(mail,
                             message =>
                             {
                                 message.IsBodyHtml = false;
                                 message.Body = mail.Message;
                             }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(SendAsync)}: Couldn't send email. To={mail.To.Email}; Subject={mail.Subject}");
                throw;
            }
        }

        public async Task SendAsHtmlAsync(EmailInfo mail)
        {
            try
            {

                await SendInternal(mail,
                                   (message) =>
                                   {
                                       message.IsBodyHtml = true;
                                       message.HtmlBody = mail.Message;
                                   }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(SendAsHtmlAsync)}: Couldn't send email. To={mail.To.Email}; Subject={mail.Subject}");
                throw;
            }
        }

        private async Task SendInternal(EmailInfo mail, Action<MailMessage> handleHtml)
        {
            var license = new License();

            var licenseInfo = _configuration.GetSection("Aspose-EmailLicense").Value;
            if (String.IsNullOrWhiteSpace(licenseInfo))
            {
                _logger.LogInformation($"{nameof(SendInternal)}: Aspose-EmailLicense value is empty");
                return;
            }
            var info = Encoding.UTF8.GetBytes(licenseInfo);
            using (var stream = new MemoryStream(info))
            {
                license.SetLicense(stream);
                _logger.LogInformation($"{nameof(SendInternal)}: Aspose-EmailLicense value is added");
            }


            using (var msg = new MailMessage())
            {
                CreateEmail(mail, msg, handleHtml);

                using (var client = new SmtpClient())
                {
                    if (_mailServiceConfig.SendGridEnabled)
                    {
                        SetSendGridClient(client);
                    }
                    else
                    {
                        client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                        client.PickupDirectoryLocation = _mailServiceConfig.LocalEmailDirectory;
                    }

                    var attachmentStreams = mail.Attachments.Select(att => new Tuple<Stream, string>(ConvertToStream(att.Content), att.Name)).ToList();
                    try
                    {
                        foreach (var namedStream in attachmentStreams)
                        {
                            msg.AddAttachment(new Attachment(namedStream.Item1, namedStream.Item2));
                        }
                        await client.SendAsync(msg).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Sending an email to {mail.To.Email} with subject '{mail.Subject}' failed");
                    }
                    finally
                    {
                        // dispose of each of the attachment streams
                        foreach (var attachFileStreamWithName in attachmentStreams)
                        {
                            attachFileStreamWithName.Item1.Dispose();
                        }

                        // dispose of each attachment
                        foreach (var attachment in msg.Attachments)
                        {
                            attachment.Dispose();
                        }
                    }
                }

                _logger.LogInformation($"Email sent. To={mail.To.Email}; Subject={mail.Subject}");
            }
        }

        private static void CreateEmail(EmailInfo mail, MailMessage msg, Action<MailMessage> handleHtml)
        {
            handleHtml(msg);
            msg.BodyEncoding = Encoding.UTF8;
            msg.From = new MailAddress(mail.From.Email, mail.From.Name);
            msg.Subject = mail.Subject;
            msg.SubjectEncoding = Encoding.UTF8;


            msg.To.Add(new MailAddress(mail.To.Email, mail.To.Name));

            if (!String.IsNullOrEmpty(mail.BccEmail))
            {
                msg.Bcc.Add(new MailAddress(mail.BccEmail));
            }

            if (!String.IsNullOrEmpty(mail.ReplyTo))
            {
                msg.ReplyToList = new MailAddress(mail.ReplyTo, mail.ReplyToName);
            }
        }

        private void SetSendGridClient(SmtpClient client)
        {
            var port = _mailServiceConfig.SendGridPort;

            client.SecurityOptions = SecurityOptions.SSLExplicit;
            client.UseAuthentication = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Host = _mailServiceConfig.SendGridHost;
            client.Password = _mailServiceConfig.SendGridPassword;
            client.Port = port;
            client.Username = _mailServiceConfig.SendGridUsername;

            if (String.IsNullOrWhiteSpace(client.Host))
            {
                Console.WriteLine("The SendGrid 'host' property was not set in the mailServiceConfig's AppSettings.");
                throw new ConfigurationException("The SendGrid 'host' property was not set in the mailServiceConfig's AppSettings.");
            }

            if (String.IsNullOrWhiteSpace(client.Password))
            {
                Console.WriteLine("The SendGrid 'password' property was not set in the mailServiceConfig's AppSettings.");
                throw new ConfigurationException("The SendGrid 'password' property was not set in the mailServiceConfig's AppSettings.");
            }

            if (String.IsNullOrWhiteSpace(client.Username))
            {
                Console.WriteLine("The SendGrid 'username' property was not set in the mailServiceConfig's AppSettings.");
                throw new ConfigurationException("The SendGrid 'username' property was not set in the mailServiceConfig's AppSettings.");
            }
        }

        private static Stream ConvertToStream(byte[] content)
        {
            var memStream = new MemoryStream();
            memStream.Write(content, 0, content.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            return memStream;
        }
    }
}