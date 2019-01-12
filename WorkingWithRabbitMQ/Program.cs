using RabbitMQ.Client;
using System;
using System.Text;

namespace WorkingWithRabbitMQ
{
    class Program
    {
        private static string url = "amqps://rsydflmj:RfVhrh7rBzjbRmgSOH12Y7HfvXCraAbB@dinosaur.rmq.cloudamqp.com/rsydflmj";
        private static bool durable = true;
        private static bool exclusive;
        private static bool autoDelete;

        // Create a ConnectionFactory and set the Uri to the CloudAMQP url
        // the connectionfactory is stateless and can safetly be a static resource in your app
        static readonly ConnectionFactory connectionFactory = new ConnectionFactory
        {
            Uri = new Uri(url),
        };

        static void Main(string[] args)
        {
            //DirectPublish1();
            //DirectPublish2();
            //SetUpFanoutExchange();
        }


        public static void DirectPublish2()
        {

            IConnection connection = connectionFactory.CreateConnection();
            IModel channel = connection.CreateModel();
            Console.WriteLine(string.Concat("Connection open: ", connection.IsOpen));

            var queueName = "my.first.queue";
            bool durable = true;
            bool exclusive = false;
            bool autoDelete = false;

            channel.ExchangeDeclare("my.first.exchange", ExchangeType.Direct, true, false, null);
            channel.QueueDeclare(queueName, durable, exclusive, autoDelete, null);
            channel.QueueBind("my.first.queue", "my.first.exchange", "");


            IBasicProperties properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.DeliveryMode = 2;
            properties.ContentType = "text/plain";
            PublicationAddress address = new PublicationAddress(ExchangeType.Direct, "my.first.exchange", "");
            channel.BasicPublish(address, properties, Encoding.UTF8.GetBytes("This is a message from the RabbitMq .NET driver"));

            channel.Close();
            connection.Close();
            Console.WriteLine(string.Concat("Channel is closed: ", channel.IsClosed));

            Console.WriteLine("Main done...");
            Console.ReadKey();

        }


        //////////////////////

        private static void DirectPublishMessage()
        {
            // create a connection and open a channel, dispose them when done
            using (var conn = connectionFactory.CreateConnection())
            using (var channel = conn.CreateModel())
            {
                for (int i = 0; i < 10; i++)
                {
                    // The message we want to put on the queue
                    var message = DateTime.Now.ToString("F");
                    // the data put on the queue must be a byte array
                    var data = Encoding.UTF8.GetBytes(message);
                    // ensure that the queue exists before we publish to it
                    var queueName = "queue1";
                    channel.QueueDeclare(queueName, durable, exclusive, autoDelete, null);
                    // publish to the "default exchange", with the queue name as the routing key
                    var exchangeName = "";
                    var routingKey = "queue1";
                    channel.BasicPublish(exchangeName, routingKey, null, data);
                }
            }
        }

        private static void GetMessage()
        {
            using (var conn = connectionFactory.CreateConnection())
            using (var channel = conn.CreateModel())
            {
                // ensure that the queue exists before we access it
                channel.QueueDeclare("queue1", true, false, false, null);
                // do a simple pull of the queue 
                var data = channel.BasicGet("queue1", false);
               
                // the message is null if the queue was empty 
                if (data == null) return;
                // convert the message back from byte[] to a string
                var message = Encoding.UTF8.GetString(data.Body);
                // ack the message, ie. confirm that we have processed it
                // otherwise it will be requeued a bit later
                channel.BasicAck(data.DeliveryTag, false);
            }
        }
    }
}
