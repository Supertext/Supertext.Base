using System;

namespace Supertext.Base.Security.Cryptography
{
    public interface IAesEncryptor : IDisposable
    {
        string Encrypt(string reference);

        string Decrypt(string encryptedReference);
    }
}
