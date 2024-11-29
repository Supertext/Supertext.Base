using System;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Security.Cryptography.AesEncryption;

namespace Supertext.Base.Security.Cryptography.Tests.AesEncryption
{
    [TestClass]
    public class AesEncryptorTest
    {
        private AesEncryptor _testee;
        private string _referenceId;
        private EncryptionConfig _config;

        [TestInitialize]
        public void TestInitialize()
        {
            _config = new EncryptionConfig() { Key = "MyEncryptionText", Iv = "IVsAreVeryNiceOk" };
            _testee = new AesEncryptor(_config);
            _referenceId = "test";
        }

        [TestMethod]
        public void Encrypt_ReferenceIsGivenAndIsEncrypted_WhenTheResultIsDecryptedItsTheSameAsTheOriginalMessage()
        {
            var encryptedReference = _testee.Encrypt(_referenceId);

            var decryptedReference = _testee.Decrypt(encryptedReference);
            decryptedReference.Should().Be(_referenceId);
        }

        [TestMethod]
        public void Encrypt_ReferenceIsGivenWithPasswordAndIsEncrypted_WhenTheResultIsDecryptedWithSamePassword_ReturnsTheSameAsTheOriginalMessage()
        {
            var userId = 1;
            var userFirstName = "Test";
            var userAge = 25;

            var encryptedReference = _testee.Encrypt(_referenceId,
                                                     userId.ToString(),
                                                     userFirstName,
                                                     userAge.ToString());

            var decryptedReference = _testee.Decrypt(encryptedReference,
                                                     userId.ToString(),
                                                     userFirstName,
                                                     userAge.ToString());
            decryptedReference.Should().Be(_referenceId);
        }

        [TestMethod]
        public void Encrypt_ReferenceIsGivenWithPasswordAndIsEncrypted_WhenTheResultIsDecryptedWithDifferentPassword_ReturnsEmptyString()
        {
            var userId = 1;
            var userFirstName = "Test";
            var wrongUserFirstName = "User";
            var userAge = 25;

            var encryptedReference = _testee.Encrypt(_referenceId,
                                                     userId.ToString(),
                                                     userFirstName,
                                                     userAge.ToString());

            var decryptedReference = _testee.Decrypt(encryptedReference,
                                                     userId.ToString(),
                                                     wrongUserFirstName,
                                                     userAge.ToString());

            decryptedReference.Should().Be("");
        }

        [TestMethod]
        public void Decrypt_IVisDifferentThanTheOneUsedForEncryption_ReturnsEmptyString()
        {
            var encryptedReference = _testee.Encrypt(_referenceId);
            _config = new EncryptionConfig() { Key = "MyEncryptionText", Iv = "ThisIsDifferent!" };
            _testee = new AesEncryptor(_config);

            var decryptedReference = _testee.Decrypt(encryptedReference);

            decryptedReference.Should().Be("");
        }

        [TestMethod]
        public void Decrypt_KeyisDifferentThanTheOneUsedForEncryption_ReturnsEmptyString()
        {
            var encryptedReference = _testee.Encrypt(_referenceId);
            _config = new EncryptionConfig() { Key = "NewEncryptionKey", Iv = "IVsAreVeryNiceOk" };
            _testee = new AesEncryptor(_config);

            var decryptedReference = _testee.Decrypt(encryptedReference);

            decryptedReference.Should().Be(String.Empty);
        }
    }
}