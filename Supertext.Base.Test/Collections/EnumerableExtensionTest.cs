using System.Collections.Generic;
using System.Collections.ObjectModel;
using Supertext.Base.Collections;

namespace Supertext.Base.Test.Collections
{
    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EnumerableExtensionTest
    {
        private ICollection<string> _testee;

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

            items.Should().Contain(new List<string> { item1, item2 });
        }

        [TestMethod]
        public void None_CollectionIsEmpty_IsNone()
        {
            var result = _testee.None();

            result.Should().BeTrue();
        }
    }
}