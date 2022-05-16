using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Exceptions;
using Supertext.Base.Extensions;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Supertext.Base.Tests.Extensions
{
    [TestClass]
    public class AttributeExtensionsTests
    {
        [TestClassAttr("my-test-value")]
        [NonInheritedTestClassAttr("my-test-value")]
        private class MyTestClass
        {
            [TestMethodAttr("my-test-method-value")]
            [NonInheritedTestMethodAttr("my-test-method-value")]
            public virtual int MyMethod()
            {
                return 0;
            }


            [TestMethodAttr("another-test-method-value")]
            [NonInheritedTestMethodAttr("another-test-method-value")]
            public virtual int AnotherMethod(int i, string s)
            {
                return 0;
            }
        }

        private class MyDerivedClass : MyTestClass
        {
            public override int MyMethod()
            {
                return 1;
            }


            public override int AnotherMethod(int i, string s)
            {
                return 1;
            }
        }

        [AttributeUsage(AttributeTargets.Class, Inherited = true)]
        private class TestClassAttrAttribute : Attribute
        {
            public string TestValue { get; }


            public TestClassAttrAttribute(string testValue)
            {
                TestValue = testValue;
            }
        }

        [AttributeUsage(AttributeTargets.Method, Inherited = true)]
        private class TestMethodAttrAttribute : Attribute
        {
            public string TestValue { get; }


            public TestMethodAttrAttribute(string testValue)
            {
                TestValue = testValue;
            }
        }

        [AttributeUsage(AttributeTargets.Class, Inherited = false)]
        private class NonInheritedTestClassAttrAttribute : Attribute
        {
            public string TestValue { get; }


            public NonInheritedTestClassAttrAttribute(string testValue)
            {
                TestValue = testValue;
            }
        }

        [AttributeUsage(AttributeTargets.Method, Inherited = false)]
        private class NonInheritedTestMethodAttrAttribute : Attribute
        {
            public string TestValue { get; }


            public NonInheritedTestMethodAttrAttribute(string testValue)
            {
                TestValue = testValue;
            }
        }

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
        private class TestEnumAttribute : Attribute
        {
            public string StringValue { get; }

            public TestEnumAttribute(string stringValue)
            {
                StringValue = stringValue;
            }
        }

        private class TestPropInfoClass
        {
            [DisplayName("Test Display Name")]
            public string DecoratedProperty { get; set; }

            public string UndecoratedProperty { get; set; }
        }

        [AttributeUsage(AttributeTargets.Field)]
        private class TestMissingEnumAttribute : Attribute
        { }

        private enum TestEnum
        {
            [TestEnum("test-value-1")]
            TestValue1,

            TestValue2
        }

        private enum MultipleTestEnum
        {
            [TestEnum("test-value-1")]
            [TestEnum("test-value-2")]
            TestValue1,

            TestValue2
        }

        [TestMethod]
        public void GetAttributeOfType_On_Decorated_PropertyInfo_Returns_Expected_Value()
        {
            // Arrange
            var propInfo = typeof(TestPropInfoClass).GetProperty(nameof(TestPropInfoClass.DecoratedProperty), BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public);
            
            // Act
            var attr = propInfo.GetAttributeOfType<DisplayNameAttribute>();

            // Assert
            attr.Should().NotBeNull();
            attr.DisplayName.Should().Be("Test Display Name");
        }

        [TestMethod]
        public void GetAttributeOfType_On_Undecorated_PropertyInfo_Returns_Expected_Value()
        {
            // Arrange
            var propInfo = typeof(TestPropInfoClass).GetProperty(nameof(TestPropInfoClass.UndecoratedProperty), BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public);

            // Act
            var attr = propInfo.GetAttributeOfType<DisplayNameAttribute>();

            // Assert
            attr.Should().BeNull();
        }

        [TestMethod]
        public void GetAttributeOfType_On_Enum_Returns_Expected_Value()
        {
            // Arrange
            const TestEnum testEnum = TestEnum.TestValue1;

            // Act
            var val = testEnum.GetAttributeOfType<TestEnumAttribute>();

            // Assert
            val.StringValue.Should().Be("test-value-1");
        }

        [TestMethod]
        public void GetAttributeOfType_Missing_On_Enum_Returns_Null()
        {
            // Arrange
            const TestEnum testEnum = TestEnum.TestValue1;

            // Act
            var val = testEnum.GetAttributeOfType<TestMissingEnumAttribute>();

            // Assert
            val.Should().BeNull();
        }

        [TestMethod]
        public void GetAttributeOfType_With_Specifier_On_Enum_Returns_Expected_Value()
        {
            // Arrange
            const TestEnum testEnum = TestEnum.TestValue1;

            // Act
            var val = testEnum.GetAttributeOfType<TestEnumAttribute, string>(f => f.StringValue);

            // Assert
            val.Should().Be("test-value-1");
        }

        [TestMethod]
        public void GetAttributeOfType_With_Specifier_On_Enum_Throws_Expected_Exception()
        {
            // Arrange
            const TestEnum testEnum = TestEnum.TestValue2;

            // Act
            try
            {
                testEnum.GetAttributeOfType<TestEnumAttribute, string>(f => f.StringValue);
            }
            catch (Exception exception)
            {
                // Assert
                exception.Message.Should().Be("The enum is not decorated with the specified attribute.");
                return;
            }

            Assert.Fail("The expected exception was not thrown.");
        }

        [TestMethod]
        public void GetAttributesOfType_On_Enum_Returns_Expected_Values()
        {
            // Arrange
            const MultipleTestEnum multipleTestEnum = MultipleTestEnum.TestValue1;

            // Act
            var vals = multipleTestEnum.GetAttributesOfType<TestEnumAttribute>();
            
            // Assert
            vals.Should().NotBeNull();
            vals.Count().Should().Be(2);
            vals.SingleOrDefault(val => val.StringValue == "test-value-1").Should().NotBeNull();
            vals.SingleOrDefault(val => val.StringValue == "test-value-2").Should().NotBeNull();
        }

        [TestMethod]
        public void GetAttributesOfType_On_Enum_Returns_Empty_Collection_When_None_Specified()
        {
            // Arrange
            const MultipleTestEnum multipleTestEnum = MultipleTestEnum.TestValue2;

            // Act
            var vals = multipleTestEnum.GetAttributesOfType<TestEnumAttribute>();

            // Assert
            vals.Should().NotBeNull();
            vals.Should().BeEmpty();
        }

        [TestMethod]
        public void GetAttributeValues_At_Class_Level_Returns_Expected_Value()
        {
            // Arrange

            // Act
            var result = typeof(MyTestClass).GetAttributeValues((TestClassAttrAttribute attr) => attr.TestValue);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be("my-test-value");
        }

        [TestMethod]
        public void GetAttributeValues_At_Class_Level_Returns_Expected_Value_From_Derived_Class()
        {
            // Arrange

            // Act
            var result = typeof(MyDerivedClass).GetAttributeValues((TestClassAttrAttribute attr) => attr.TestValue, true);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be("my-test-value");
        }

        [TestMethod]
        public void GetAttributeValues_At_Class_Level_Throws_Expected_Excpn_From_Derived_Class_Without_Attribute_Inheritance()
        {
            // first prove that the attribute is obtainable on the base class

            // Act
            var result = typeof(MyTestClass).GetAttributeValues((NonInheritedTestClassAttrAttribute attr) => attr.TestValue, true);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be("my-test-value");


            // now prove that the attribute is unobtainable on the derived class

            // Act
            try
            {
                typeof(MyDerivedClass).GetAttributeValues((NonInheritedTestClassAttrAttribute attr) => attr.TestValue, true).ToList();
            }
            catch (AttributeNotFoundException)
            {
                return;
            }

            // Assert
            Assert.Fail("The expected exception was not thrown.");
        }

        [TestMethod]
        public void GetAttributeValues_At_Method_Level_Returns_Expected_Value()
        {
            // Arrange
            var type = typeof(MyTestClass);
            const string methodName = nameof(MyTestClass.MyMethod);

            // Act
            var result = type.GetAttributeValues(methodName,
                                                 (TestMethodAttrAttribute attr) => attr.TestValue);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be("my-test-method-value");
        }

        [TestMethod]
        public void GetAttributeValues_At_Method_Level_Returns_Expected_Value_From_Derived_Class()
        {
            // Arrange
            var type = typeof(MyDerivedClass);
            const string methodName = nameof(MyDerivedClass.MyMethod);

            // Act
            var result = type.GetAttributeValues(methodName,
                                                 (TestMethodAttrAttribute attr) => attr.TestValue,
                                                 false,
                                                 true);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be("my-test-method-value");
        }

        [TestMethod]
        public void GetAttributeValues_At_Method_Level_Throws_Expected_Excpn_From_Derived_Class_Without_Attribute_Inheritance()
        {
            // first prove that the attribute is obtainable on the base class

            // Arrange
            var type = typeof(MyTestClass);
            const string methodName = nameof(MyTestClass.MyMethod);

            // Act
            var result = type.GetAttributeValues(methodName,
                                                 (NonInheritedTestMethodAttrAttribute attr) => attr.TestValue,
                                                 false,
                                                 true);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be("my-test-method-value");


            // now prove that the attribute is unobtainable on the derived class

            // Act
            try
            {
                typeof(MyDerivedClass).GetAttributeValues(methodName,
                                                          (NonInheritedTestMethodAttrAttribute attr) => attr.TestValue,
                                                          true)
                                      .ToList();
            }
            catch (AttributeNotFoundException)
            {
                return;
            }

            // Assert
            Assert.Fail("The expected exception was not thrown.");
        }

        [TestMethod]
        public void GetAttributeValues_At_Method_Level_Throws_Expected_Excpn_From_Derived_Class_With_Attribute_Inheritance_But_Specified_To_Ignore_Inheritance()
        {
            // first prove that the attribute is obtainable on the base class

            // Arrange
            var type = typeof(MyTestClass);
            const string methodName = nameof(MyTestClass.MyMethod);

            // Act
            var result = type.GetAttributeValues(methodName,
                                                 (TestMethodAttrAttribute attr) => attr.TestValue,
                                                 false,
                                                 false);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be("my-test-method-value");


            // now prove that the attribute is unobtainable on the derived class

            // Act
            try
            {
                typeof(MyDerivedClass).GetAttributeValues(nameof(MyDerivedClass.MyMethod),
                                                          (TestMethodAttrAttribute attr) => attr.TestValue,
                                                          false,
                                                          false)
                                      .ToList();
            }
            catch (AttributeNotFoundException)
            {
                return;
            }

            // Assert
            Assert.Fail("The expected exception was not thrown.");
        }

        [TestMethod]
        public void GetAttributeValues_At_Method_Level_Throws_Expected_Excpn_When_Specifying_No_Param_Types_For_Method_With_Params()
        {
            // Arrange
            var type = typeof(MyTestClass);
            const string methodName = nameof(MyTestClass.AnotherMethod);

            // Act
            try
            {
                type.GetAttributeValues(methodName,
                                        new Type[0],
                                        (TestMethodAttrAttribute attr) => attr.TestValue)
                    .ToList();
            }
            catch (ArgumentException)
            {
                return;
            }

            // Assert
            Assert.Fail("The expected exception was not thrown.");
        }

        [TestMethod]
        public void GetAttributeValues_At_Method_Level_Throws_Expected_Excpn_When_Specifying_Invalid_Param_Type_For_Method_With_Params()
        {
            // Arrange
            var type = typeof(MyTestClass);
            const string methodName = nameof(MyTestClass.AnotherMethod);

            // Act
            try
            {
                type.GetAttributeValues(methodName,
                                        new[] {typeof(long)},
                                        (TestMethodAttrAttribute attr) => attr.TestValue)
                    .ToList();
            }
            catch (ArgumentException)
            {
                return;
            }

            // Assert
            Assert.Fail("The expected exception was not thrown.");
        }

        [TestMethod]
        public void GetAttributeValues_At_Method_Level_When_Specifying_Param_Types_Returns_Expected_Value()
        {
            // Arrange
            var type = typeof(MyTestClass);
            const string methodName = nameof(MyTestClass.AnotherMethod);

            // Act
            var result = type.GetAttributeValues(methodName,
                                                 new[] {typeof(int), typeof(string)},
                                                 (TestMethodAttrAttribute attr) => attr.TestValue);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be("another-test-method-value");
        }

        [TestMethod]
        public void GetAttributeValues_At_Method_Level_When_Specifying_Type_Returns_Expected_Value_From_Derived_Class()
        {
            // Arrange
            var type = typeof(MyDerivedClass);
            const string methodName = nameof(MyDerivedClass.AnotherMethod);

            // Act
            var result = type.GetAttributeValues(methodName,
                                                 new[] {typeof(int), typeof(string)},
                                                 (TestMethodAttrAttribute attr) => attr.TestValue,
                                                 false,
                                                 true);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be("another-test-method-value");
        }

        [TestMethod]
        public void GetAttributeValues_At_Method_Level_When_Specifying_Type_Throws_Expected_Excpn_From_Derived_Class_Without_Attribute_Inheritance()
        {
            // first prove that the attribute is obtainable on the base class

            // Arrange
            var type = typeof(MyTestClass);
            const string methodName = nameof(MyTestClass.AnotherMethod);

            // Act
            var result = type.GetAttributeValues(methodName,
                                                 new[] {typeof(int), typeof(string)},
                                                 (NonInheritedTestMethodAttrAttribute attr) => attr.TestValue,
                                                 false,
                                                 true);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be("another-test-method-value");


            // now prove that the attribute is unobtainable on the derived class

            // Act
            try
            {
                typeof(MyDerivedClass).GetAttributeValues(methodName,
                                                          new[] {typeof(int), typeof(string)},
                                                          (NonInheritedTestMethodAttrAttribute attr) => attr.TestValue,
                                                          true)
                                      .ToList();
            }
            catch (AttributeNotFoundException)
            {
                return;
            }

            // Assert
            Assert.Fail("The expected exception was not thrown.");
        }

        [TestMethod]
        public void GetAttributeValues_At_Method_Level_When_Specifying_Type_Throws_Expected_Excpn_From_Derived_Class_With_Attribute_Inheritance_But_Specified_To_Ignore_Inheritance()
        {
            // first prove that the attribute is obtainable on the base class

            // Arrange
            var type = typeof(MyTestClass);
            const string methodName = nameof(MyTestClass.AnotherMethod);

            // Act
            var result = type.GetAttributeValues(methodName,
                                                 new[] {typeof(int), typeof(string)},
                                                 (TestMethodAttrAttribute attr) => attr.TestValue,
                                                 false,
                                                 false);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Should().Be("another-test-method-value");


            // now prove that the attribute is unobtainable on the derived class

            // Act
            try
            {
                typeof(MyDerivedClass).GetAttributeValues(methodName,
                                                          new[] {typeof(int), typeof(string)},
                                                          (TestMethodAttrAttribute attr) => attr.TestValue,
                                                          false,
                                                          false).ToList();
            }
            catch (AttributeNotFoundException)
            {
                return;
            }

            // Assert
            Assert.Fail("The expected exception was not thrown.");
        }
    }
}