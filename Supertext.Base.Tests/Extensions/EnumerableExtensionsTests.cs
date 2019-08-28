using Supertext.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;


namespace Supertext.Base.Tests.Extensions
{
    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;


    [TestClass]
    public class EnumerableExtensionsTests
    {
        private ICollection<string> _testee;


        private struct TestPerson
        {
            public string Name { get; set; }


            public TestPerson(string name)
            {
                Name = name;
            }
        }


        [TestInitialize]
        public void TestInitialize()
        {
            _testee = new Collection<string>();
        }


        [TestMethod]
        public void IsEmpty_CollectionIsEmpty_IsEmpty()
        {
            var result = _testee.IsEmpty();

            result.Should().BeTrue();
        }


        [TestMethod]
        public void IsEmpty_CollectionIsFilled_IsNotEmpty()
        {
            _testee.Add("Anything");

            var result = _testee.IsEmpty();

            result.Should().BeFalse();
        }


        [TestMethod]
        public void ForEach_CollectionIsFilled_EachItemIsTouched()
        {
            var item1 = "one";
            var item2 = "one";
            _testee.Add(item1);
            _testee.Add(item2);
            var items = new Collection<string>();

            _testee.ForEach(item => items.Add(item));

            items.Should().Contain(new List<string> {item1, item2});
        }


        [TestMethod]
        public void IsNullOrEmpty_Returns_False_For_NonNull_Source()
        {
            // Arrange
            var source = new List<object>
                             {
                                 new object()
                             };

            // Act
            var result = source.IsNullOrEmpty();

            // Assert
            Assert.IsFalse(result);
        }


        [TestMethod]
        public void IsNullOrEmpty_Returns_True_For_Empty_Source()
        {
            // Arrange
            var source = new List<object>(0);

            // Act
            var result = source.IsNullOrEmpty();

            // Assert
            Assert.IsTrue(result);
        }


        [TestMethod]
        public void IsNullOrEmpty_Returns_True_For_Null_Source()
        {
            // Arrange
            List<object> source = null;

            // Act
            var result = source.IsNullOrEmpty();

            // Assert
            Assert.IsTrue(result);
        }


        [TestMethod]
        public void None_CollectionIsEmpty_IsNone()
        {
            var result = _testee.None();

            result.Should().BeTrue();
        }


        [TestMethod]
        public void ToCommaSeparatedStringWithQuotes_Returns_Expected_String_With_Default_Separator()
        {
            // Arrange
            var source = new List<string>
                             {
                                 "Bart",
                                 "Homer",
                                 "Lisa",
                                 "Maggie",
                                 "Marge"
                             };
            const string expectedResult = "\"Bart\", \"Homer\", \"Lisa\", \"Maggie\", \"Marge\"";

            // Act
            var result = source.ToCommaSeparatedStringWithQuotes();

            // Assert
            Assert.AreEqual(expectedResult, result);
        }


        [TestMethod]
        public void ToCommaSeparatedStringWithQuotes_Returns_Expected_String_With_Specified_Separator()
        {
            // Arrange
            var source = new List<string>
                             {
                                 "Bart",
                                 "Homer",
                                 "Lisa",
                                 "Maggie",
                                 "Marge"
                             };
            const string expectedResult = "\"Bart\" | \"Homer\" | \"Lisa\" | \"Maggie\" | \"Marge\"";

            // Act
            var result = source.ToCommaSeparatedStringWithQuotes(" | ");

            // Assert
            Assert.AreEqual(expectedResult, result);
        }


        [TestMethod]
        public void ToCommaSeparatedStringWithQuotes_Returns_Null_For_Null_Source()
        {
            // Arrange
            List<string> source = null;

            // Act
            var result = source.ToCommaSeparatedStringWithQuotes();

            // Assert
            Assert.IsNull(result);
        }


        [TestMethod]
        public void ToCommaSeparatedStringWithQuotes_Returns_Empty_String_For_Empty_Source()
        {
            // Arrange
            var source = new List<string>(0);

            // Act
            var result = source.ToCommaSeparatedStringWithQuotes();

            // Assert
            Assert.AreEqual(String.Empty, result);
        }


        [TestMethod]
        public void IndexOf_Returns_Expected_Result()
        {
            // Arrange
            var tpMarge = new TestPerson("Marge");
            IEnumerable<TestPerson> source = new List<TestPerson>
                                                 {
                                                     new TestPerson("Bart"),
                                                     new TestPerson("Homer"),
                                                     new TestPerson("Lisa"),
                                                     new TestPerson("Maggie"),
                                                     tpMarge
                                                 };

            // Act
            var result = source.IndexOf(tpMarge);

            // Assert
            result.Should().Be(source.Count() - 1);
        }


        [TestMethod]
        public void IndexOf_Returns_NotFound_For_Empty_Source()
        {
            // Arrange
            var tpMarge = new TestPerson("Marge");
            IEnumerable<TestPerson> source = new List<TestPerson>(0);

            // Act
            var result = source.IndexOf(tpMarge);

            // Assert
            result.Should().Be(-1);
        }


        [TestMethod]
        public void IndexOf_Throws_Exception_For_Null_Source()
        {
            // Arrange
            var tpMarge = new TestPerson("Marge");
            IEnumerable<TestPerson> source = null;

            // Act
            try
            {
                source.IndexOf(tpMarge);
            }
            catch (ArgumentNullException)
            {
                return;
            }

            // Assert
            Assert.Fail("The expected exception was not thrown.");
        }


        [TestMethod]
        public void ItemWithMax_Returns_Expected_Item()
        {
            // Arrange
            var tpMarge = new TestPerson("Marge");
            IEnumerable<TestPerson> source = new List<TestPerson>
                                                 {
                                                     new TestPerson("Homer"),
                                                     new TestPerson("Lisa"),
                                                     tpMarge,
                                                     new TestPerson("Bart"),
                                                     new TestPerson("Maggie")
                                                 };

            // Act
            var result = source.ItemWithMax(tp => tp.Name);

            // Assert
            Assert.AreEqual(tpMarge, result);
        }


        [TestMethod]
        public void ItemWithMax_Throws_Exception_For_Null_Source()
        {
            // Arrange
            IEnumerable<TestPerson> source = null;

            // Act
            try
            {
                source.ItemWithMax(tp => tp.Name);
            }
            catch (ArgumentNullException exception) when(exception.ParamName == "source")
            {
                // Assert
                return;
            }

            Assert.Fail("The expected exception was not thrown.");
        }


        [TestMethod]
        public void ItemWithMax_Throws_Exception_For_Empty_Source()
        {
            // Arrange
            IEnumerable<TestPerson> source = new List<TestPerson>(0);

            // Act
            try
            {
                source.ItemWithMax(tp => tp.Name);
            }
            catch (InvalidOperationException)
            {
                // Assert
                return;
            }

            Assert.Fail("The expected exception was not thrown.");
        }


        [TestMethod]
        public void MoveToFirst_Moves_Expected_Single_Item()
        {
            // Arrange
            var tpMarge = new TestPerson("Marge");
            IEnumerable<TestPerson> source = new List<TestPerson>
                                                 {
                                                     new TestPerson("Homer"),
                                                     new TestPerson("Lisa"),
                                                     tpMarge,
                                                     new TestPerson("Bart"),
                                                     new TestPerson("Maggie")
                                                 };

            // Act
            var result = source.MoveToFirst(tp => tp.Name == tpMarge.Name);

            // Assert
            Assert.AreEqual(tpMarge, result.First());
        }


        [TestMethod]
        public void MoveToFirst_Moves_Expected_Items()
        {
            // Arrange
            var tpMaggie = new TestPerson("Maggie");
            var tpMarge = new TestPerson("Marge");
            IEnumerable<TestPerson> source = new List<TestPerson>
                                                 {
                                                     new TestPerson("Homer"),
                                                     new TestPerson("Lisa"),
                                                     tpMarge,
                                                     new TestPerson("Bart"),
                                                     tpMaggie
                                                 };

            // Act
            var result = source.MoveToFirst(tp => tp.Name.StartsWith("M"));

            // Assert
            Assert.AreEqual(tpMaggie, result.First());
            Assert.AreEqual(tpMarge, result.ElementAt(1));
        }


        [TestMethod]
        public void MoveToFirst_Throws_Exception_For_Null_Source()
        {
            // Arrange
            IEnumerable<TestPerson> source = null;

            // Act
            try
            {
                source.MoveToFirst(tp => tp.Name == "whatever");
            }
            catch (ArgumentNullException exception) when (exception.ParamName == "source")
            {
                // Assert
                return;
            }

            Assert.Fail("The expected exception was not thrown.");
        }
    }
}