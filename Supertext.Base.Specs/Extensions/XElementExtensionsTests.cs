using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Extensions;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Supertext.Base.Specs.Extensions
{
    [TestClass]
    public class XElementExtensionsTests
    {
        private static readonly Random Rdm = new Random();

        [TestMethod]
        public void GetAttrValue_Retrieves_Expected_Boolean_Value()
        {
            // Arrange
            const bool val = true;
            var elmnt = new XElement("test", new XAttribute("attr", val));

            // Act
            var result = elmnt.GetAttrValue<bool>("attr");

            // Assert
            result.GetType().Should().Be(typeof(Boolean));
            result.Should().Be(val);
        }

        [TestMethod]
        public void GetAttrValue_Retrieves_Expected_Int_Value()
        {
            // Arrange
            var val = Rdm.Next();
            var elmnt = new XElement("test", new XAttribute("attr", val));

            // Act
            var result = elmnt.GetAttrValue<int>("attr");

            // Assert
            result.GetType().Should().Be(typeof(int));
            result.Should().Be(val);
        }

        [TestMethod]
        public void GetAttrValue_Retrieves_Expected_String_Value()
        {
            // Arrange
            var val = Guid.NewGuid().ToString();
            var elmnt = new XElement("test", new XAttribute("attr", val));

            // Act
            var result = elmnt.GetAttrValue<string>("attr");

            // Assert
            result.GetType().Should().Be(typeof(string));
            result.Should().Be(val);
        }

        [TestMethod]
        public void GetAttrValue_Returns_Default_Boolean_Value_For_Missing_Attribute()
        {
            // Arrange
            const bool defaultVal = default(bool);
            var elmnt = new XElement("test");

            // Act
            var result = elmnt.GetAttrValue<bool>("attr");

            // Assert
            result.GetType().Should().Be(typeof(Boolean));
            result.Should().Be(defaultVal);
        }

        [TestMethod]
        public void GetAttrValue_Returns_Default_Int_Value_For_Missing_Attribute()
        {
            // Arrange
            const int defaultVal = default(int);
            var elmnt = new XElement("test");

            // Act
            var result = elmnt.GetAttrValue<int>("attr");

            // Assert
            result.GetType().Should().Be(typeof(int));
            result.Should().Be(defaultVal);
        }

        [TestMethod]
        public void GetAttrValue_Returns_Default_String_Value_For_Missing_Attribute()
        {
            // Arrange
            const string defaultVal = default(string);
            var elmnt = new XElement("test");

            // Act
            var result = elmnt.GetAttrValue<string>("attr");

            // Assert
            result.Should().Be(defaultVal);
        }

        [TestMethod]
        public void GetAttrValue_Returns_Specified_Default_Boolean_Value_For_Missing_Attribute()
        {
            // Arrange
            const bool defaultVal = true;
            var elmnt = new XElement("test");

            // Act
            var result = elmnt.GetAttrValue("attr", defaultVal);

            // Assert
            result.GetType().Should().Be(typeof(Boolean));
            result.Should().Be(defaultVal);
        }

        [TestMethod]
        public void GetAttrValue_Returns_Specified_Default_Int_Value_For_Missing_Attribute()
        {
            // Arrange
            var defaultVal = Rdm.Next();
            var elmnt = new XElement("test");

            // Act
            var result = elmnt.GetAttrValue("attr", defaultVal);

            // Assert
            result.GetType().Should().Be(typeof(int));
            result.Should().Be(defaultVal);
        }

        [TestMethod]
        public void GetAttrValue_Returns_Specified_Default_String_Value_For_Missing_Attribute()
        {
            // Arrange
            var defaultVal = Guid.NewGuid().ToString();
            var elmnt = new XElement("test");

            // Act
            var result = elmnt.GetAttrValue("attr", defaultVal);

            // Assert
            result.GetType().Should().Be(typeof(string));
            result.Should().Be(defaultVal);
        }

        [TestMethod]
        public void GetAttrValue_Throws_Expected_Exception_For_Null_XElement()
        {
            // Arrange
            XElement elmnt = null;

            // Act
            try
            {
                elmnt.GetAttrValue<string>("whatever");
            }
            catch (ArgumentNullException)
            {
                return;
            }

            // Assert
            Assert.Fail("The expected exception was not thrown.");
        }

        [TestMethod]
        public void GetAttrValue_Throws_Expected_Exception_For_Null_attrName()
        {
            // Arrange
            var elmnt = new XElement("test", new XAttribute("attr", Rdm.Next()));

            // Act
            try
            {
                elmnt.GetAttrValue<string>(null);
            }
            catch (ArgumentException)
            {
                return;
            }

            // Assert
            Assert.Fail("The expected exception was not thrown.");
        }

        [TestMethod]
        public void GetAttrValue_Throws_Expected_Exception_For_Empty_attrName()
        {
            // Arrange
            var elmnt = new XElement("test", new XAttribute("attr", Rdm.Next()));

            // Act
            try
            {
                elmnt.GetAttrValue<string>(String.Empty);
            }
            catch (ArgumentException)
            {
                return;
            }

            // Assert
            Assert.Fail("The expected exception was not thrown.");
        }

        [TestMethod]
        public void RemoveAllNamespaces_Returns_Expected_When_Passed_XDocument()
        {
            // Arrange
            var resources = new Base.Resources.EmbeddedResource();
            var xmlContents = resources.ReadContentsAsString("Supertext.Base.Specs.Extensions.TestFiles.Xml_with_namespace.sdlproj");
            var xElmnt = XDocument.Parse(xmlContents);

            void AssertHasDeclaration(XDocument xElmntAssert)
            {
                xElmntAssert.Document.Declaration.Should().NotBeNull();
            }

            // Act
            AssertHasDeclaration(xElmnt);
            AssertHasNamespaces(xElmnt.Root);
            var modifiedXElmnt = xElmnt.RemoveAllNamespaces();

            // Assert
            AssertHasDeclaration(xElmnt);
            modifiedXElmnt.Root.Should().NotBeNull();
            AssertNoNamespaces(modifiedXElmnt.Root);
            AssertNoXmlnsAttr(modifiedXElmnt.Root);
        }

        [TestMethod]
        public void RemoveAllNamespaces_Returns_Expected_When_Passed_XElement()
        {
            // Arrange
            var resources = new Base.Resources.EmbeddedResource();
            var xmlContents = resources.ReadContentsAsString("Supertext.Base.Specs.Extensions.TestFiles.Xml_with_namespace.sdlproj");
            var xElmnt = XElement.Parse(xmlContents);

            // Act
            AssertHasNamespaces(xElmnt);
            var modifiedXElmnt = xElmnt.RemoveAllNamespaces();

            // Assert
            modifiedXElmnt.Should().NotBeNull();
            AssertNoNamespaces(modifiedXElmnt);
            AssertNoXmlnsAttr(modifiedXElmnt);
        }

        private static void AssertHasNamespaces(XElement xElmntAssert)
        {
            xElmntAssert.Name.ToString().Should().NotBe(xElmntAssert.Name.LocalName);
            xElmntAssert.Name.Namespace.NamespaceName.Should().NotBe(String.Empty);

            foreach (var xChildElmntAssert in xElmntAssert.Elements())
            {
                AssertHasNamespaces(xChildElmntAssert);
            }
        }

        private static void AssertNoNamespaces(XElement xElmntAssert)
        {
            xElmntAssert.Name.ToString().Should().Be(xElmntAssert.Name.LocalName);
            xElmntAssert.Name.Namespace.NamespaceName.Should().Be(String.Empty);

            foreach (var xChildElmntAssert in xElmntAssert.Elements())
            {
                AssertNoNamespaces(xChildElmntAssert);
            }
        }

        private static void AssertNoXmlnsAttr(XElement xElmntAssert)
        {
            xElmntAssert.Attributes().Where(attr => attr.IsNamespaceDeclaration).Should().BeEmpty();
            xElmntAssert.Attributes().Where(attr => attr.Name.LocalName.StartsWith("xmlns")).Should().BeEmpty();

            foreach (var xChildElmntAssert in xElmntAssert.Elements())
            {
                AssertNoXmlnsAttr(xChildElmntAssert);
            }
        }
    }
}