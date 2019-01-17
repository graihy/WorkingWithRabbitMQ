using MassTransit;
using MassTransit.RabbitMqTransport;
using System;
using System.Threading.Tasks;

namespace ECR.MessageQueueRepository
{
    /// <summary>
    /// contains commonly used functions to be executed on RabbitMQ
    /// </summary>
    public abstract class RabbitMQBase
    {
        protected string rabbitMqAddress;
        protected IBusControl senderBus;

        /// <summary>
        /// Generic method used to send message to specific queues (exchange type is Direct)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="queueName"></param>
        public void Send<T>(object message, string queueName) where T : class
        {
            try
            {
                Task<ISendEndpoint> sendEndpointTask = senderBus.GetSendEndpoint(new Uri(string.Concat(rabbitMqAddress, "/", queueName)));
                ISendEndpoint sendEndpoint = sendEndpointTask.Result;
                Task t = sendEndpoint.Send<T>(message, context =>
                {
                    (context as RabbitMqSendContext).Mandatory = true;
                });
                t.Wait();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        ///  Generic method used to send message to all queues on this exchange (exchange type is Fan-out)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        public void Publish<T>(object values) where T : class
        {
            senderBus.Publish<T>(values);
        }
    }
}