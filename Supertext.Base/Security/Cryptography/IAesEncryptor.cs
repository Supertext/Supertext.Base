using System;

namespace Supertext.Base.Security.Cryptography
{
    public interface IAesEncryptor : IDisposable
    {
        string Encrypt(string reference);

        string Encrypt(string text, params string[] passwordInputs);

        string Decrypt(string encryptedReference);

        string Decrypt(string encryptedReference, params string[] passwordInputs);
    }
}
