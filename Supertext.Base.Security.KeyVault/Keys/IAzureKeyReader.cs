using System.Security.Cryptography;

namespace Supertext.Base.Security.KeyVault.Keys
{
    internal interface IAzureKeyReader
    {
        string ReadPrivateKey(RSA privateKey);
    }
}