using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Supertext.Base.Common;

namespace Supertext.Base.Messaging.MassTransit
{
    internal class MessageConsumer<TMessage> : IConsumer<TMessage> where TMessage : class
    {
        private readonly ICollection<IMessageConsumer<TMessage>> _consumers;
        private readonly ILogger<MessageConsumer<TMessage>> _logger;

        public MessageConsumer(ICollection<IMessageConsumer<TMessage>> consumers, ILogger<MessageConsumer<TMessage>> logger)
        {
            _consumers = consumers;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            _logger.LogDebug($"Consuming message of type {typeof(TMessage).Name} with correlation ID {context.CorrelationId}.");
            var consumerTasks = new List<Task>();
            foreach (var consumer in _consumers)
            {
                var correlationId = context.CorrelationId.HasValue
                                               ? Option<Guid>.Some(context.CorrelationId.Value)
                                               : Option<Guid>.None();
                var consumerTask = consumer.HandleAsync(context.Message,
                                                        correlationId,
                                                        context.CancellationToken);

                consumerTasks.Add(consumerTask);
            }

            await Task.WhenAll(consumerTasks);
        }
    }
}