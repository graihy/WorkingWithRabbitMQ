using ECR.MessageQueueRepository;
using System;

namespace MassTransit.Publisher
{
    public class SingletonMassTransit : RabbitMQBase, IDisposable
    {
        private static SingletonMassTransit _instance;
        private readonly IBusControl _rabbitBusControl;
        private readonly string maroofRabbitMqAddress;

        private SingletonMassTransit()
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

        public static SingletonMassTransit Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SingletonMassTransit();
                }
                return _instance;
            }
        }

        public void Dispose()
        {

            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            //GC.SuppressFinalize(this);

        }
        public void Dispose(bool isSafeDispose)
        {
            //Free unmanaged resources
            if (_rabbitBusControl != null)
            {
                _rabbitBusControl.Stop();
            }

            // Free managed resources too, but only if I'm being called from Dispose
            //(If I'm being called from Finalize then the objects might not exist anymore
            if (isSafeDispose)
            {
            }
        }


        //~MaroofRabbitMQSingleton()
        //{
        //    Dispose(false); //I am *not* calling you from Dispose, it's *not* safe
        //}
    }
}