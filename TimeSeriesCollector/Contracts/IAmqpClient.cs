using RabbitMQ.Client.Events;
using System;

namespace TimeSeriesCollector.Contracts
{
    public interface IAmqpClient
    {
        public void SubscribeToEvents(Action<string, BasicDeliverEventArgs> action);

        public void AcknowledgeMessage(ulong deliveryTag);

        public void RejectMessage(ulong deliveryTag);
    }
}
