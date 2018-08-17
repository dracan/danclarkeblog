using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace DanClarkeBlog.Core.Helpers
{
    [UsedImplicitly]
    public class RabbitMqMessageQueue : IMessageQueue
    {
        private readonly IConnection _connection;
        private readonly List<IModel> _channels = new List<IModel>();

        public RabbitMqMessageQueue(Settings settings)
        {
            var factory = new ConnectionFactory {
                HostName = settings.RabbitMQServer,
                DispatchConsumersAsync = true
            };

            factory.UserName = settings.RabbitMQUser;
            factory.Password = settings.RabbitMQPass;

            _connection = factory.CreateConnection();
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _channels.ForEach(x => x.Dispose());
        }

        public void Send(string queueName, string message)
        {
            if(!_channels.Any())
            {
                _channels.Add(_connection.CreateModel());
            }

            _channels[0].QueueDeclare(queueName, false, false, false, null); // Ensure Queue Exists
            _channels[0].BasicPublish("", queueName, null, Encoding.UTF8.GetBytes(message));
            Log.Debug("Sent message: {Message}", message);
        }

        public void Subscribe(string queueName, Func<string, Task> callbackAsync)
        {
            var channel = _connection.CreateModel();
            _channels.Add(channel);

            channel.QueueDeclare(queueName, false, false, false, null); // Ensure Queue Exists

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"Messsage received for queue {queueName}");

                await callbackAsync(message);
            };

            channel.BasicConsume(queueName, true, consumer);
        }
    }
}