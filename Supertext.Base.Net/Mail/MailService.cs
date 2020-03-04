using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Xsl;
using Aspose.Email;
using Aspose.Email.Clients;
using Aspose.Email.Clients.Smtp;
using Microsoft.Extensions.Logging;

namespace Supertext.Base.Net.Mail
{
    public class MailService
    {
        private static ILogger<MailService> _logger;
        private MailServiceConfig _configuration;

        public MailService(ILogger<MailService> logger, MailServiceConfig configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public List<AttachmentInfo> Attachments { set; get; } = new List<AttachmentInfo>();

        public string ReplyTo { get; set; }

        public string ReplyToName { get; set; }

        public string Subject { set; get; }

        public string Message { set; get; }

        private Dictionary<string, string> TemplateEntries { get; } = new Dictionary<string, string>();

        public void Send(string fromName,
                         string fromEmail,
                         string toName,
                         string toEmail,
                         string bccEmail = null)
        {
            try
            {
                Send_Internal(false,
                              fromName,
                              fromEmail,
                              toName,
                              toEmail,
                              bccEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Send)}: Couldn't send email. To={toEmail}; Subject={Subject}", ex);
                //throw ex; Really can't test it like this.
            }
        }

        public void SendAsHtml(string fromName,
                               string fromEmail,
                               string toName,
                               string toEmail,
                               string bccEmail = null)
        {
            try
            {
                Send_Internal(true,
                              fromName,
                              fromEmail,
                              toName,
                              toEmail,
                              bccEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(SendAsHtml)}: Couldn't send email. To={toEmail}; Subject={Subject}", ex);
                //throw ex; Really can't test it like this.
            }
        }

        public void SetSubjectAndBodyFromTemplate(Stream templateStream)
        {
            Subject = BuildMailContent(TemplateEntries, "Subject", templateStream);
            Message = BuildMailContent(TemplateEntries, "Message", templateStream);
        }

        public void SetTemplateEntries(IEnumerable<KeyValuePair<string, string>> templateEntries)
        {
            foreach (var item in templateEntries)
            {
                TemplateEntries.Add(item.Key, item.Value);
            }
        }

        private void Send_Internal(bool asHtml,
                                   string fromName,
                                   string fromEmail,
                                   string toName,
                                   string toEmail,
                                   string bccEmail = null)
        {
            using (var msg = new MailMessage())
            {
                msg.IsBodyHtml = asHtml;
                msg.BodyEncoding = System.Text.Encoding.UTF8;
                msg.From = new MailAddress(fromEmail, fromName);
                msg.Subject = Subject;
                msg.SubjectEncoding = System.Text.Encoding.UTF8;

                if (asHtml)
                {
                    msg.HtmlBody = Message;
                }
                else
                {
                    msg.Body = Message;
                }

                msg.To.Add(new MailAddress(toEmail, toName));

                if (!String.IsNullOrEmpty(bccEmail))
                {
                    msg.Bcc.Add(new MailAddress(bccEmail));
                }

                if (!String.IsNullOrEmpty(ReplyTo))
                {
                    msg.ReplyToList = new MailAddress(ReplyTo, ReplyToName);
                }

                using (var client = new SmtpClient())
                {
                    if (_configuration.SendGridEnabled)
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
                            throw new ConfigurationErrorsException("The SendGrid 'host' property was not set in the configuration's AppSettings.");
                        }

                        if (String.IsNullOrWhiteSpace(client.Password))
                        {
                            Console.WriteLine("The SendGrid 'password' property was not set in the configuration's AppSettings.");
                            throw new ConfigurationErrorsException("The SendGrid 'password' property was not set in the configuration's AppSettings.");
                        }

                        if (String.IsNullOrWhiteSpace(client.Username))
                        {
                            Console.WriteLine("The SendGrid 'username' property was not set in the configuration's AppSettings.");
                            throw new ConfigurationErrorsException("The SendGrid 'username' property was not set in the configuration's AppSettings.");
                        }
                    }
                    else
                    {
                        client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                        client.PickupDirectoryLocation = _configuration.PickupDirectory;
                    }

                    //TODO: Find a better way to get the Attachments
                    /*
                     * Idea: get file from Byte[] and add it to Attachments
                     * need to understand how they converted the MemoryStream to an Attachment
                    */


                    var attachFileStreamsWithNames = Attachments.Select(att => new Tuple<Stream, string>(ConvertToFileStream(att.Content), att.Name)).ToList();

                    try
                    {
                        // build a collection of tuples containing the attachment stream and name

                        // foreach of these attachment tuples create an Attachment object and add it to the email object
                        attachFileStreamsWithNames.ForEach(attachment => msg.AddAttachment(new Attachment(attachment.Item1, attachment.Item2)));

                        // send the email
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
                    }

                }
            }

            _logger.LogInformation($"Email sent. To={toEmail}; Subject={Subject}");
        }

        private static FileStream ConvertToFileStream(byte[] content)
        {
            var memStream = new MemoryStream();
            var binForm = new BinaryFormatter();
            memStream.Write(content, 0, content.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            return (FileStream)binForm.Deserialize(memStream);
        }

        private static string BuildMailContent(Dictionary<string, string> replaceEntries, string mailType, Stream template)
        {
            try
            {
                var xmlDoc = new XmlDocument();

                var element = xmlDoc.CreateElement(mailType);
                xmlDoc.AppendChild(element);

                foreach (var key in replaceEntries.Keys)
                {
                    element = xmlDoc.CreateElement(key);
                    element.InnerText = replaceEntries[key];
                    xmlDoc.DocumentElement?.AppendChild(element);
                }

                var transForm = new XslCompiledTransform();
                var settings = new XmlReaderSettings {DtdProcessing = DtdProcessing.Parse};

                using (var reader = XmlReader.Create(template, settings))
                {
                    transForm.Load(reader);

                    using (var sw = new StringWriter())
                    {
                        var nav = xmlDoc.CreateNavigator();

                        transForm.Transform(nav, null, sw);

                        return sw.ToString();
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError($"{nameof(BuildMailContent)}:", exception);
                throw;
            }
            finally
            {
                template.Position = 0;
            }
        }
    }
}