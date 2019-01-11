using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMq.OneWayMessage.Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            ReceiveSingleOneWayMessage();
        }

        private static void ReceiveSingleOneWayMessage()
        {
            IConnection connection = GetConnectionFactoryUsingURI().CreateConnection();
            IModel channel = connection.CreateModel();

            //sets up the basic behaviour of message handling.
            //one message at a time  and we don’t want to process any additional messages until the actual one has been processed.
            //the prefetch size, sets the maximum size of for the messages fetched from the queue where 0 means there is no upper limit.
            //prefetch count, is the number of messages to be fetched from the queue at a time.
            //The boolean “global” parameter is set to false which means that the prefetch limits are valid for the current consumer only, not for the entire connection
            channel.BasicQos(0, 1, false);
            DefaultBasicConsumer basicConsumer = new OneWayMessageReceiver(channel);
            channel.BasicConsume("my.first.queue", false, basicConsumer);
            // is this added

        }

        private static ConnectionFactory GetConnectionFactoryUsingURI()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.Uri = new Uri("amqps://rsydflmj:RfVhrh7rBzjbRmgSOH12Y7HfvXCraAbB@dinosaur.rmq.cloudamqp.com/rsydflmj");
            return connectionFactory;
        }



    }
}
