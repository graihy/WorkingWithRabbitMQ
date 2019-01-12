using MyCompany.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.Publisher
{
    class Program
    {
        public static object RegisterNewOrderConsumer { get; private set; }

        static void Main(string[] args)
        {
            RunMassTransitPublisherWithRabbit();
        }

        private static void RunMassTransitPublisherWithRabbit()
        {
            string rabbitMqAddress = "rabbitmq://dinosaur.rmq.cloudamqp.com/rsydflmj";
            string rabbitMqQueue = "mycompany.domains.queues";
            Uri rabbitMqRootUri = new Uri(rabbitMqAddress);
            //setup a RabbitMQ bus instance
            IBusControl rabbitBusControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
            {
                rabbit.Host(rabbitMqRootUri, settings =>
                {
                    settings.Password("RfVhrh7rBzjbRmgSOH12Y7HfvXCraAbB");
                    settings.Username("rsydflmj");
                });
            });

            Task<ISendEndpoint> sendEndpointTask = rabbitBusControl.GetSendEndpoint(new Uri(string.Concat(rabbitMqAddress, "/", rabbitMqQueue)));
            ISendEndpoint sendEndpoint = sendEndpointTask.Result;

            Task sendTask = sendEndpoint.Send<IRegisterCustomer>(new
            {
                Address = "New Street",
                Id = Guid.NewGuid(),
                Preferred = true,
                RegisteredUtc = DateTime.UtcNow,
                Name = "Nice people LTD",
                Type = 1,
                DefaultDiscount = 0
            });
            Console.ReadKey();
        }
    }
}