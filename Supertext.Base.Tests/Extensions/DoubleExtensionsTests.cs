using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Extensions;


namespace Supertext.Base.Tests.Extensions
{
    [TestClass]
    public class DoubleExtensionsTests
    {
        [TestMethod]
        public void RoundToNearestHalf()
        {
            var expected = System.Double.MinValue;
            var actual = (System.Double.MinValue + 0.1D).RoundToNearestHalf();
            Assert.IsTrue(System.Double.Equals(expected, actual));

            expected = System.Double.MinValue;
            actual = (System.Double.MinValue + 0.25D).RoundToNearestHalf();
            Assert.IsTrue(System.Double.Equals(expected, actual));

            expected = System.Double.MinValue + 0.5D;
            actual = (System.Double.MinValue + 0.26D).RoundToNearestHalf();
            Assert.IsTrue(System.Double.Equals(expected, actual));

            expected = System.Double.MaxValue;
            actual = (System.Double.MaxValue - 0.1D).RoundToNearestHalf();
            Assert.IsTrue(System.Double.Equals(expected, actual));

            expected = System.Double.MaxValue;
            actual = (System.Double.MaxValue - 0.25D).RoundToNearestHalf();
            Assert.IsTrue(System.Double.Equals(expected, actual));

            expected = System.Double.MaxValue - 0.5D;
            actual = (System.Double.MaxValue - 0.26D).RoundToNearestHalf();
            Assert.IsTrue(System.Double.Equals(expected, actual));

            expected = 0D;
            actual = 0D.RoundToNearestHalf();
            Assert.IsTrue(System.Double.Equals(expected, actual));

            expected = 0.5D;
            actual = 0.25D.RoundToNearestHalf();
            Assert.IsTrue(System.Double.Equals(expected, actual));

            expected = 0.5D;
            actual = 0.26D.RoundToNearestHalf();
            Assert.IsTrue(System.Double.Equals(expected, actual));

            expected = -0.5D;
            actual = -0.25D.RoundToNearestHalf();
            Assert.IsTrue(System.Double.Equals(expected, actual));
        }
    }
}