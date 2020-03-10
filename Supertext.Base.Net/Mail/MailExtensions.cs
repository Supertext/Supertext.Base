using System;
using System.Text;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace Supertext.Base.Net.Mail
{
    public static class MailExtensions
    {
        /// <summary>
        /// Registers Aspose Email License. Configure section 'Aspose-EmailLicense' in appsettings.json
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddAsposeMailLicense(this IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration) {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var license = new Aspose.Email.License();

            try
            {
                // Initializes a license from a stream
                var licenseInfo = configuration.GetSection("Aspose-EmailLicense").Value;
                var info = Encoding.UTF8.GetBytes(licenseInfo);
                using (var stream = new MemoryStream(info))
                {
                    license.SetLicense(stream);
                    Console.WriteLine("Aspose Email License set successfully.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"There was an error setting the Aspose Email License. Exception Message: {e.Message}");
                throw;
            }
        }
    }
}