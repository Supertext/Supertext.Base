using System;
using Autofac;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Dal.SqlServer.Modules;
using Supertext.Base.Modules;

namespace Supertext.Base.Dal.SqlServer.Specs
{
    [TestClass]
    public class UnitOfWorkFactoryTest
    {
        private IUnitOfWorkFactory _testee;
        private ContainerBuilder _builder;

        [TestInitialize]
        public void TestInitialize()
        {
            _builder = new ContainerBuilder();
            _builder.RegisterModule<SqlServerModule>();
            _builder.RegisterModule<BaseModule>();

            var container = _builder.Build();
            _testee = container.Resolve<IUnitOfWorkFactory>();
        }

        [TestMethod]
        public void Create_ConnectionStringIsValid_UnitOfWorkIsCreated()
        {
            // Act
            var result = _testee.Create("someConnectionString");

            // Assert
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void Create_ConnectionStringIsNull_AnExceptionIsThrown()
        {
            // Assert
            _testee.Invoking(testee => testee.Create(null))
                   .Should()
                   .Throw<ArgumentException>();
        }

        [TestMethod]
        public void CreateU_ConnectionStringIsBlank_AnExceptionIsThrown()
        {
            // Assert
            _testee.Invoking(testee => testee.Create(" "))
                   .Should()
                   .Throw<ArgumentException>();
        }

        [TestMethod]
        public void Create_ConnectionStringIsEmpty_AnExceptionIsThrown()
        {
            // Assert
            _testee.Invoking(testee => testee.Create(""))
                   .Should()
                   .Throw<ArgumentException>();
        }
    }
}