using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Supertext.Base.Security.Cryptography.AesEncryptor
{
    public class AesEncryptor : IAesEncryptor
    {
        private static ILogger<IAesEncryptor> _logger;
        private readonly AesCryptoServiceProvider _cryptoServiceProvider;
        private const int InputOffset = 0;

        public AesEncryptor(EncryptionConfig config, ILogger<IAesEncryptor> logger)
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
            _logger = logger;
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
                _logger.LogError($"Error decrypting Reference {encryptedText}. Key or IV may be different.");
            }
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        public void Dispose()
        {
            _cryptoServiceProvider?.Dispose();
        }
    }
}