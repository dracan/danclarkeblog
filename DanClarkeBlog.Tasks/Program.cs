using System;
using System.Threading;
using Autofac;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Tasks.Tasks;
using Polly;
using RabbitMQ.Client.Exceptions;

namespace DanClarkeBlog.Tasks
{
    internal static class Program
    {
        static void Main()
        {
            try
            {
                var container = TasksBootstrapper.Init();

                var connectionFailedMessageLogged = false;

                var retryPolicy = Policy.Handle<BrokerUnreachableException>().OrInner<BrokerUnreachableException>()
                    .WaitAndRetryForever(_ => TimeSpan.FromSeconds(30),
                        (ex, timespan) =>
                    {
                        if (connectionFailedMessageLogged) return;
                        Console.WriteLine("Failed to connect. Will keep retrying forever until a connection is successful.");
                        connectionFailedMessageLogged = true;
                    });

                Console.WriteLine("Starting to listen ...");

                retryPolicy.Execute(() =>
                {
                    using(var queue = container.Resolve<IMessageQueue>())
                    {
                        queue.Subscribe("sync", container.Resolve<SyncTask>().ExecuteAsync);

                        Thread.Sleep(Timeout.Infinite);
                    }
                });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}