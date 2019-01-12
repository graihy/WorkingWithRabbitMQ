using MassTransit.RabbitMqTransport;
using System;

namespace MassTransit.Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            RunMassTransitReceiverWithRabbit();
        }

        private static void RunMassTransitReceiverWithRabbit()
        {
            //setup a RabbitMQ bus instance to configure aspects of the bus
            IBusControl rabbitBusControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
            {
                //configure the RabbitMQ host settings
                IRabbitMqHost rabbitMqHost = rabbit.Host(new Uri("rabbitmq://dinosaur.rmq.cloudamqp.com/rsydflmj"), settings =>
                {
                    settings.Password("RfVhrh7rBzjbRmgSOH12Y7HfvXCraAbB");
                    settings.Username("rsydflmj");
                });

                rabbit.ReceiveEndpoint(rabbitMqHost, "mycompany.masstransit.domains.queues", conf =>
                {
                    conf.Consumer<RegisterCustomerConsumer>();
                });
            });

            rabbitBusControl.Start();
            Console.ReadKey();

            rabbitBusControl.Stop();
        }
    }
}