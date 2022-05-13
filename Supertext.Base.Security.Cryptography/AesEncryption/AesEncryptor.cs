using System;
using System.Security.Cryptography;
using System.Text;

namespace Supertext.Base.Security.Cryptography.AesEncryption
{
    internal class AesEncryptor : IAesEncryptor
    {
        private readonly Aes _aes;
        private const int InputOffset = 0;

        public AesEncryptor(EncryptionConfig config)
        {
            _aes = Aes.Create();
            _aes.BlockSize = 128;
            _aes.KeySize = 128;
            _aes.Mode = CipherMode.CBC;
            _aes.Padding = PaddingMode.PKCS7;
            _aes.Key = Encoding.UTF8.GetBytes(config.Key);
            _aes.IV = Encoding.UTF8.GetBytes(config.Iv);
        }

        public string Encrypt(string text)
        {
            var transform = _aes.CreateEncryptor();
            var encryptedBytes = transform.TransformFinalBlock(Encoding.UTF8.GetBytes(text), InputOffset, text.Length);

            return Convert.ToBase64String(encryptedBytes);
        }

        public string Decrypt(string encryptedText)
        {
            var transform = _aes.CreateDecryptor();
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
            _aes?.Dispose();
        }
    }
}