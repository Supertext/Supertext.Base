using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Extensions;
using System;
using System.Linq;
using System.Xml.Linq;


namespace Supertext.Base.Test.Extensions
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
        public void RemoveAllNamespaces_Returns_Expected()
        {
            // Arrange
            var resouces = new Base.Resources.EmbeddedResource();
            var xmlContents = resouces.ReadContentsAsString("Supertext.Base.Test.Extensions.TestFiles.Xml_with_namespace.sdlproj");
            var xElmnt = XElement.Parse(xmlContents);

            // Act
            var modifiedXelmnnt = xElmnt.RemoveAllNamespaces();

            // Assert
            modifiedXelmnnt.Should().NotBeNull();
            modifiedXelmnnt.ToString().Should().NotContain("ps:");
            modifiedXelmnnt.Attributes().Count(attr => attr.IsNamespaceDeclaration).Should().Be(0);
        }
    }
}