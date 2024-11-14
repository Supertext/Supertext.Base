using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Extensions;
using System.Collections.Generic;

namespace Supertext.Base.Tests.Extensions;

[TestClass]
public class TypeExtensions
{
    [TestMethod]
    public void IsDictionary_WhenInstanceIsIDictionaryOfStringString_ReturnsTrue()
    {
        IDictionary<string, string> testee = new Dictionary<string, string>();

        testee.GetType().IsDictionary().Should().BeTrue();
    }

    [TestMethod]
    public void IsDictionary_WhenTypeIsIDictionaryOfStringString_ReturnsTrue()
    {
        typeof(IDictionary<string, string>).IsDictionary().Should().BeTrue();
    }
}