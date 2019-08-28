using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Common;

namespace Supertext.Base.Tests.Common
{
    [TestClass]
    public class StringTruncatorTest
    {
        [TestMethod]
        public void Truncate_StringWithLengthOf20TruncatedTo45_NoTruncation()
        {
            const int length = 45;
            const string text = "01234567890123456789";

            var result = StringTruncator.Truncate(text, length);

            result.Should().Be(text);
        }

        [TestMethod]
        public void Truncate_StringWithLengthOf20TruncatedTo10_Truncation()
        {
            const int length = 10;
            const string text = "01234567890123456789";

            var result = StringTruncator.Truncate(text, length);

            result.Should().Be("0123456789");
        }

        [TestMethod]
        public void TruncateWithPaddingRight_StringWithLengthOf20TruncatedTo45_NoTruncation()
        {
            const int length = 45;
            const string text = "01234567890123456789";

            var result = StringTruncator.TruncateWithPaddingRight(text, length);

            result.Should().Be(text);
        }

        [TestMethod]
        public void TruncateWithPaddingRight_StringWithLengthOf20TruncatedTo10_TruncationWithPadding()
        {
            const int length = 10;
            const string text = "01234567890123456789";

            var result = StringTruncator.TruncateWithPaddingRight(text, length);

            result.Should().Be("01234567..");
        }
    }
}