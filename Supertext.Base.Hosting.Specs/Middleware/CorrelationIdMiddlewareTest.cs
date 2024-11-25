using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Supertext.Base.Hosting.Specs.Middleware.Infrastructure;

namespace Supertext.Base.Hosting.Specs.Middleware
{
    [TestClass]
    public class CorrelationIdMiddlewareTest
    {
        private const string CorrelationIdHeader = "x-correlation-id";
        private HttpClient? _testee;
        private CorrelationHandlingApplication? _application;

        [TestInitialize]
        public void TestInitialize()
        {
            Log.Logger = new LoggerConfiguration()
                         .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {CorrelationId} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                         .Enrich.FromLogContext()
                         .Enrich.WithCorrelationIdHeader()
                         .CreateLogger();

            _application = new CorrelationHandlingApplication();
            _testee = _application.CreateClient();
        }

        [TestMethod]
        public async Task GetAsync_InvokedWithoutCorrelationIdInHeader_ItIsCreated()
        {
            var response = await _testee!.GetAsync("/api/test/4711");

            var result = await response.Content.ReadAsStringAsync();

            result.Should().Be(Startup.Guid.ToLowerInvariant());
        }

        [TestMethod]
        public async Task Get_InvokedWithCorrelationIdInHeader_ItIsUsed()
        {
            const string correlationId = "0c651a923ebb420e8a323b2656d33d1c";
            _testee!.DefaultRequestHeaders.Add(CorrelationIdHeader, correlationId);

            var response = await _testee!.GetAsync("/api/test/4711");

            var result = await response.Content.ReadAsStringAsync();

            result.Should().Be(Guid.Parse(correlationId).ToString());
        }

        [TestMethod]
        public async Task Get_InvokedWithCorrelationIdInHeaderNotReformatted_ItIsUsed()
        {
            const string correlationId = "B6B03A9F-2D0E-4BA4-A47D-FFFC38AB1079";
            _testee!.DefaultRequestHeaders.Add(CorrelationIdHeader, correlationId);

            var response = await _testee!.GetAsync("/api/test/4711");

            var result = await response.Content.ReadAsStringAsync();

            result.Should().Be(Guid.Parse(correlationId).ToString());
        }

        [TestMethod]
        public async Task GetAsync_InvokedWithoutCorrelationIdInHeader_CorrelationIdIsInResponseHeader()
        {
            var response = await _testee!.GetAsync("/api/test/4711");

            var correlationId = response.Headers.GetValues(CorrelationIdHeader).Single();

            correlationId.Should().Be(Guid.Parse(Startup.Guid).ToString().ToLowerInvariant());
        }
    }
}