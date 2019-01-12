using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMq.OneWayMessage.Receiver
{
    class Program
    {
        private static IModel channelForEventing;
        static void Main(string[] args)
        {
            //ReceiveSingleOneWayMessage();
            ReceiveMessagesWithEvents();

            Console.WriteLine("Main done...");
            Console.ReadKey();
        }

        private static ConnectionFactory GetConnectionFactoryUsingURI()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.Uri = new Uri("amqps://rsydflmj:RfVhrh7rBzjbRmgSOH12Y7HfvXCraAbB@dinosaur.rmq.cloudamqp.com/rsydflmj");
            return connectionFactory;
        }


        #region using DefaultBasicConsumer for subscribing 
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
        }
        #endregion using DefaultBasicConsumer for subscribing 




        #region using EventingBasicConsumer for subscribing 
        private static void ReceiveMessagesWithEvents()
        {
            IConnection connection = GetConnectionFactoryUsingURI().CreateConnection();
            channelForEventing = connection.CreateModel();
            channelForEventing.BasicQos(0, 1, false);
            //exposes the message handling functions as events
            EventingBasicConsumer eventingBasicConsumer = new EventingBasicConsumer(channelForEventing);
            
            //eventingBasicConsumer.Received += EventingBasicConsumer_Received;

            eventingBasicConsumer.Received += (sender, basicDeliveryEventArgs) =>
            {
                IBasicProperties basicProperties = basicDeliveryEventArgs.BasicProperties;
                Console.WriteLine("Message received by the event based consumer. Check the debug window for details.");
                Debug.WriteLine(string.Concat("Message received from the exchange ", basicDeliveryEventArgs.Exchange));
                Debug.WriteLine(string.Concat("Content type: ", basicProperties.ContentType));
                Debug.WriteLine(string.Concat("Consumer tag: ", basicDeliveryEventArgs.ConsumerTag));
                Debug.WriteLine(string.Concat("Delivery tag: ", basicDeliveryEventArgs.DeliveryTag));
                Debug.WriteLine(string.Concat("Message: ", Encoding.UTF8.GetString(basicDeliveryEventArgs.Body)));
                //channelForEventing.BasicNack(basicDeliveryEventArgs.DeliveryTag, false,true);
                channelForEventing.BasicAck(basicDeliveryEventArgs.DeliveryTag, false);
            };
            channelForEventing.BasicConsume("mycompany.queues.accounting", false, eventingBasicConsumer);
        }

        //exposes the message handling functions as events
        private static void EventingBasicConsumer_Received(object sender, BasicDeliverEventArgs e)
        {
            IBasicProperties basicProperties = e.BasicProperties;
            Console.WriteLine("Message received by the event based consumer. Check the debug window for details.");
            Debug.WriteLine(string.Concat("Message received from the exchange ", e.Exchange));
            Debug.WriteLine(string.Concat("Content type: ", basicProperties.ContentType));
            Debug.WriteLine(string.Concat("Consumer tag: ", e.ConsumerTag));
            Debug.WriteLine(string.Concat("Delivery tag: ", e.DeliveryTag));
            Debug.WriteLine(string.Concat("Message: ", Encoding.UTF8.GetString(e.Body)));
            channelForEventing.BasicAck(e.DeliveryTag, false);
        }
        #endregion using EventingBasicConsumer for subscribing 



    }
}
