using System.Security.Cryptography.X509Certificates;

namespace Supertext.Base.Security.Cryptography
{
    public interface ICertificateService
    {
        X509Certificate2 GetCertificateFromKeyVault(string vaultCertificateName);
    }
}