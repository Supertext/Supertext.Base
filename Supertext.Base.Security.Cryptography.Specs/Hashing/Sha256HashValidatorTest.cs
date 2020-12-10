using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Security.Cryptography.Hashing;
using Supertext.Base.Security.Hashing;

namespace Supertext.Base.Security.Cryptography.Tests.Hashing
{
    [TestClass]
    public class Sha256HashValidatorTest
    {
        private ISha256InternalHasher _sha256Hasher;
        private Sha256HashValidator _testee;
        private HashingConfig _encryptionConfig;
        private readonly string _salt = "Salt";
        private readonly string _expectedHash = "Expected Hash";

        [TestInitialize]
        public void TestInitialize()
        {
            _encryptionConfig = new HashingConfig()
                                   {
                                       TokenHashingPepper = nameof(HashingConfig.TokenHashingPepper),
                                       PasswordHashingPepper = nameof(HashingConfig.PasswordHashingPepper)
            };
            _sha256Hasher = A.Fake<ISha256InternalHasher>();
            _testee = new Sha256HashValidator(_encryptionConfig, _sha256Hasher);
        }

        [TestMethod]
        public void IsTokenValid_TokenCorrespondsHash_ReturnsTrue()
        {
            var token = "Token";
            var expectedHashingResult = new HashingResult
                                        {
                                            HashedValue = _expectedHash,
                                            Salt = _salt
                                        };

            A.CallTo(() => _sha256Hasher.ComputeSha256HashWithSaltAndPepper(token, _salt, _encryptionConfig.TokenHashingPepper)).Returns(expectedHashingResult);

            var isValid = _testee.IsTokenValid(token, _salt, _expectedHash);

            isValid.Should().Be(true);
        }

        [TestMethod]
        public void IsTokenValid_TokenDoesNotCorrespondHash_ReturnsFalse()
        {
            var token = "Token";
            var mismatchingHash = "Mismatching Hash";
            var mismatchingHashingResult = new HashingResult
                                           {
                                               HashedValue = mismatchingHash,
                                               Salt = _salt
            };

            A.CallTo(() => _sha256Hasher.ComputeSha256HashWithSaltAndPepper(token, _salt, _encryptionConfig.TokenHashingPepper)).Returns(mismatchingHashingResult);

            var isValid = _testee.IsTokenValid(token, _salt, _expectedHash);

            isValid.Should().Be(false);
        }

        [TestMethod]
        public void IsTokenValid_PasswordCorrespondsHash_ReturnsTrue()
        {
            var password = "Password123";
            var expectedHashingResult = new HashingResult
                                        {
                                            HashedValue = _expectedHash,
                                            Salt = _salt
            };

            A.CallTo(() => _sha256Hasher.ComputeSha256HashWithSaltAndPepper(password, _salt, _encryptionConfig.TokenHashingPepper)).Returns(expectedHashingResult);

            var isValid = _testee.IsTokenValid(password, _salt, _expectedHash);

            isValid.Should().Be(true);
        }

        [TestMethod]
        public void IsPasswordValid_PasswordDoesNotCorrespondHash_ReturnsFalse()
        {
            var password = "Password123";
            var mismatchingHash = "Mismatching Hash";
            var mismatchingHashingResult = new HashingResult
                                        {
                                            HashedValue = mismatchingHash,
                                            Salt = _salt
            };

            A.CallTo(() => _sha256Hasher.ComputeSha256HashWithSaltAndPepper(password, _salt, _encryptionConfig.TokenHashingPepper)).Returns(mismatchingHashingResult);

            var isValid = _testee.IsTokenValid(password, _salt, _expectedHash);

            isValid.Should().Be(false);
        }
    }
}
