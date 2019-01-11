﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace RabbitMqNetTests
{
    class Program
    {
        static void Main(string[] args)
        {
            // IConnection object is thread safe and can be shared among multiple applications
            IConnection connection = GetConnectionFactoryUsingURI().CreateConnection();
            Console.WriteLine(string.Concat("Connection open: ", connection.IsOpen));

            IModel channel = connection.CreateModel();
            channel.ExchangeDeclare("my.first.exchange", ExchangeType.Direct, true, false, null);
            channel.QueueDeclare("my.first.queue", true, false, false, null);
            channel.QueueBind("my.first.queue", "my.first.exchange", "");

            IBasicProperties properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "text/plain";
            PublicationAddress address = new PublicationAddress(ExchangeType.Fanout, "my.first.exchange", "aaa");
            channel.BasicPublish(address, properties, Encoding.UTF8.GetBytes("This is a message from the RabbitMq .NET driver"));
            //channel.BasicPublish("my.first.exchange", "", null, Encoding.UTF8.GetBytes("This is a message from the RabbitMq .NET driver bbb"));
            
            channel.Close();
            connection.Close();
            Console.WriteLine(string.Concat("Channel is closed: ", channel.IsClosed));

            Console.WriteLine("Main done...");
            Console.ReadKey();
        }




        private static ConnectionFactory GetConnectionFactory()
        {
            //The ConnectionFactory object will help us build an IConnection object which represents a connection to the RabbitMq message broker
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.Port = 5672;
            connectionFactory.HostName = "localhost";
            connectionFactory.UserName = "accountant";
            connectionFactory.Password = "accountant";
            connectionFactory.VirtualHost = "accounting";
            return connectionFactory;
        }

        private static ConnectionFactory GetConnectionFactoryUsingURI()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.Uri = new Uri("amqps://rsydflmj:RfVhrh7rBzjbRmgSOH12Y7HfvXCraAbB@dinosaur.rmq.cloudamqp.com/rsydflmj");
            return connectionFactory;
        }
    }
}
