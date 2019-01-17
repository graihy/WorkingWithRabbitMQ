using ECR.MessageQueueRepository;
using System;

namespace MassTransit.Publisher
{
    public class MaroofRabbitMQSingleton : RabbitMQBase, IDisposable
    {
        private static MaroofRabbitMQSingleton _instance;
        private readonly IBusControl _rabbitBusControl;
        private readonly string maroofRabbitMqAddress;

        private MaroofRabbitMQSingleton()
        {
            maroofRabbitMqAddress = "rabbitmq://dinosaur.rmq.cloudamqp.com/rsydflmj";
            string maroofRabbitMqUserName = "rsydflmj";
            string maroofRabbitMqPassword = "FmJoL3n6Z78uj4rUA3RmGcV8JN6SbZ1C";
            Uri rabbitMqRootUri = new Uri(maroofRabbitMqAddress);

            _rabbitBusControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
            {
                rabbit.Host(rabbitMqRootUri, settings =>
                {
                    settings.Username(maroofRabbitMqUserName);
                    settings.Password(maroofRabbitMqPassword);
                });
            });

            senderBus = _rabbitBusControl;
            _rabbitBusControl.Start();
            rabbitMqAddress = maroofRabbitMqAddress;
        }

        public static MaroofRabbitMQSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MaroofRabbitMQSingleton();
                }
                return _instance;
            }
        }

        public void Dispose()
        {
            if (_rabbitBusControl != null)
            {
                _rabbitBusControl.Stop();

            }
        }
    }
}