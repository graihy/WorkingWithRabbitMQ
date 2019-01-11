using RabbitMQ.Client;
using System;
using System.Diagnostics;
using System.Text;

namespace RabbitMq.OneWayMessage.Receiver
{
    class OneWayMessageReceiver : DefaultBasicConsumer
    {
        private readonly IModel _channel;

        public OneWayMessageReceiver(IModel channel)
        {
            _channel = channel;
        }


        // is this worked with git 333
        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            Console.WriteLine("Message received by the consumer. Check the debug window for details.");
            Debug.WriteLine(string.Concat("Message received from the exchange ", exchange));
            Debug.WriteLine(string.Concat("Content type: ", properties.ContentType));
            Debug.WriteLine(string.Concat("Consumer tag: ", consumerTag));
            Debug.WriteLine(string.Concat("Delivery tag: ", deliveryTag));
            Debug.WriteLine(string.Concat("Message: ", Encoding.UTF8.GetString(body)));
            _channel.BasicAck(deliveryTag, false);
        }
    }
}