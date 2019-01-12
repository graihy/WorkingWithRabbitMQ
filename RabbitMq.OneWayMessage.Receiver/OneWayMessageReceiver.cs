using RabbitMQ.Client;
using System;
using System.Diagnostics;
using System.Text;

namespace RabbitMq.OneWayMessage.Receiver
{
    class OneWayMessageReceiver : DefaultBasicConsumer
    {
        //channel is used later in HandleBasicDeliver function for acknowledgement 
        private readonly IModel _channel;

        public OneWayMessageReceiver(IModel channel)
        {
            _channel = channel;
        }
        /// <summary>
        /// delivering new message
        /// </summary>
        /// <param name="consumerTag">The consumer tag is a unique ID on the message such as “amq.ctag-qCDfYIYQEpGqvAY7t-bhCQ”</param>
        /// <param name="deliveryTag"></param>
        /// <param name="redelivered"></param>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="properties"></param>
        /// <param name="body"></param>
        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            Console.WriteLine("Message received by the consumer. Check the debug window for details.");
            Debug.WriteLine(string.Concat("Message received from the exchange ", exchange));
            Debug.WriteLine(string.Concat("Content type: ", properties.ContentType));
            Debug.WriteLine(string.Concat("Consumer tag: ", consumerTag));
            Debug.WriteLine(string.Concat("Delivery tag: ", deliveryTag));
            Debug.WriteLine(string.Concat("Message: ", Encoding.UTF8.GetString(body)));

            //The delivery tag is an integer and is used for acknowledging a message.
            //When RabbitMq has received the acknowledgement then the message is deleted from the queue.
            //This tag usually indicates the position of the message in the queue
            _channel.BasicAck(deliveryTag, false);
        }
    }
}