using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Text;
using System;
using Microsoft.Extensions.Configuration;

namespace Supertext.Base.Hosting.Extensions;

public static class AsposeActivationExtension
{
    public static void AddAsposeEmailLicense(this IHost host)
    {
        var license = new Aspose.Email.License();

        try
        {
            var configuration = host.Services.GetService<IConfiguration>();
            var licenseInfo = configuration.GetSection("Aspose-EmailLicense").Value;
            if (String.IsNullOrWhiteSpace(licenseInfo))
            {
                return;
            }
            var info = Encoding.UTF8.GetBytes(licenseInfo);
            using var stream = new MemoryStream(info);
            license.SetLicense(stream);
            Console.WriteLine("Aspose Email License set successfully.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"There was an error setting the Aspose Email License. Exception Message: {e.Message}");
            throw;
        }
    }
}