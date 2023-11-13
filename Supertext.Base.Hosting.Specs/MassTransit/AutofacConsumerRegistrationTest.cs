using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supertext.Base.Hosting.MassTransit;
using Supertext.Base.Hosting.Specs.MassTransit.Consumers;
using Supertext.Base.Hosting.Specs.MassTransit.Messages;
using Supertext.Base.Messaging;

namespace Supertext.Base.Hosting.Specs.MassTransit;

[TestClass]
public class AutofacConsumerRegistrationTest
{
    [TestMethod]
    public async Task PublishMessage_TwoConsumers_SuccessfullyConsumed()
    {
        var consumerHelper = new ConsumerHelper();
        // Map the default name
        EndpointConvention.Map<TestMessage1>(new Uri("queue:TestMessage1"));

        var host = Host.CreateDefaultBuilder()
                             .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                             .ConfigureServices(services =>
                                                    services.AddMassTransitTestHarness(configurator =>
                                                                                       {
                                                                                           configurator.RegisterMessage<TestMessage1>();
                                                                                       }))
                             .ConfigureWebHostDefaults(webBuilder =>
                                                           webBuilder
                                                               .UseTestServer()
                                                               .UseStartup<Startup>())
                             .ConfigureContainer<ContainerBuilder>(containerBuilder =>
                                                                       containerBuilder.RegisterMessageConsumers(GetType().Assembly)
                                                                                       .RegisterInstance(consumerHelper))
                             .Build();

        await host.StartAsync();


        var harness = host.Services.GetTestHarness();

        var publisher = host.Services.GetService<IMessagePublisher>();

        var message = new TestMessage1 { Id = 4711 };
        await publisher!.PublishAsync(message, CancellationToken.None);

        await harness.Consumed.Any<TestMessage1>();

        await host.StopAsync();

        consumerHelper.Consumer1TestMessage1!.Id.Should().Be(4711);
        consumerHelper.Consumer2TestMessage1!.Id.Should().Be(4711);
    }

    [TestMethod]
    public async Task PublishMessage_WithCorrelationId_SuccessfullyConsumed()
    {
        var consumerHelper = new ConsumerHelper();
        // Map the default name
        EndpointConvention.Map<TestMessage2>(new Uri("queue:TestMessage2"));

        var host = Host.CreateDefaultBuilder()
                             .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                             .ConfigureServices(services =>
                                                    services.AddMassTransitTestHarness(configurator =>
                                                                                       {
                                                                                           configurator.RegisterMessage<TestMessage2>();
                                                                                       }))
                             .ConfigureWebHostDefaults(webBuilder =>
                                                           webBuilder
                                                               .UseTestServer()
                                                               .UseStartup<Startup>())
                             .ConfigureContainer<ContainerBuilder>(containerBuilder =>
                                                                       containerBuilder.RegisterMessageConsumers(GetType().Assembly)
                                                                                       .RegisterInstance(consumerHelper))
                             .Build();

        await host.StartAsync();


        var harness = host.Services.GetTestHarness();

        var publisher = host.Services.GetService<IMessagePublisher>();

        var correlationId = Guid.NewGuid();
        var message = new TestMessage2 { Id = 4711 };
        await publisher!.PublishAsync(message, correlationId, CancellationToken.None);

        await harness.Consumed.Any<TestMessage2>();

        await host.StopAsync();

        consumerHelper.CorrelationId.IsSome.Should().BeTrue();
        consumerHelper.CorrelationId.Value.Should().Be(correlationId);
    }
}