using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Supertext.Base.Core.Configuration;
using Supertext.Base.Dal.SqlServer.Utils;

namespace Supertext.Base.Dal.SqlServer.Tests.Utils;

[TestClass]
public class DalExtensionsTest
{
    private const string ExpectedConnectionString = "Data Source=.;Initial Catalog=myDatabase;Integrated Security=False;User ID=myUser;MultipleActiveResultSets=True;Connect Timeout=500";
    private IHostEnvironment _environment;
    private IConfigurationRoot _configuration;

    [TestInitialize]
    public void TestInitialize()
    {
        _environment = A.Fake<IHostEnvironment>();
        A.CallTo(() => _environment.ContentRootPath).Returns(AppDomain.CurrentDomain.BaseDirectory);
        A.CallTo(() => _environment.EnvironmentName).Returns("Development");

        var configurationBuilder = new ConfigurationBuilder()
                                   .SetBasePath(_environment.ContentRootPath)
                                   .AddJsonFile("appsettings.json")
                                   .AddEnvironmentVariables();
        _configuration = configurationBuilder.Build();
    }

    [TestMethod]
    public void CreateConnectionStringBuilder_ConfigWithConnectionGiven_BuilderWithConnectionStringCreated()
    {
        var config = _configuration.CreateConfiguredSettingsInstance<TestConfig>();

        var builder = config.CreateConnectionStringBuilder(testConfig => testConfig.ConnectionString);

        builder.ConnectionString.Should().Be(ExpectedConnectionString);
    }

    [TestMethod]
    public void CreateConnectionStringBuilder_EmptyConnectionGiven_BuilderWithEmptyConnectionString()
    {
        var config = new TestConfig();

        var builder = config.CreateConnectionStringBuilder(testConfig => testConfig.ConnectionString);

        builder.ConnectionString.Should().BeEmpty();
    }

    [TestMethod]
    public void BuilderConnectionString_MultipleActiveResultSetsChanged_ConnectionStringAsExpected()
    {
        var adaptedConnectionString = ExpectedConnectionString.Replace("MultipleActiveResultSets=True", "MultipleActiveResultSets=False");

        var builder = new SqlConnectionStringBuilder(adaptedConnectionString);

        builder.MultipleActiveResultSets = true;

        builder.ConnectionString.Should().Be(ExpectedConnectionString);
    }
}