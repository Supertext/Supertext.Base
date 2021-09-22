using System;
using System.Security.Cryptography;
using System.Text;

namespace Supertext.Base.Security.Cryptography.AesEncryption
{
    internal class AesEncryptor : IAesEncryptor
    {
        private readonly AesCryptoServiceProvider _cryptoServiceProvider;
        private const int InputOffset = 0;

        public AesEncryptor(EncryptionConfig config)
        {
            _cryptoServiceProvider = new AesCryptoServiceProvider
                                     {
                                         BlockSize = 128,
                                         KeySize = 128,
                                         Mode = CipherMode.CBC,
                                         Padding = PaddingMode.PKCS7,
                                         Key = Encoding.UTF8.GetBytes(config.Key),
                                         IV = Encoding.UTF8.GetBytes(config.Iv)
                                     };
        }

        public string Encrypt(string text)
        {
            var transform = _cryptoServiceProvider.CreateEncryptor();
            var encryptedBytes = transform.TransformFinalBlock(Encoding.UTF8.GetBytes(text), InputOffset, text.Length);

            return Convert.ToBase64String(encryptedBytes);
        }

        public string Decrypt(string encryptedText)
        {
            var transform = _cryptoServiceProvider.CreateDecryptor();
            var encryptedBytes = Convert.FromBase64String(encryptedText);
            var decryptedBytes = Convert.FromBase64String(String.Empty);
            try
            {
                decryptedBytes = transform.TransformFinalBlock(encryptedBytes, InputOffset, encryptedBytes.Length);
            }
            catch
            {
                //ILogger isn't used here because of problems with ILogger and .Net Framework
                Console.WriteLine($"Error decrypting Reference {encryptedText}. Key or IV may be different.");
            }
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        public void Dispose()
        {
            _cryptoServiceProvider?.Dispose();
        }
    }
}