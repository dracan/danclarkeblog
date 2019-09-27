using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Tasks.Tasks;

namespace DanClarkeBlog.Tasks
{
    internal static class Program
    {
        static async Task Main()
        {
            try
            {
                var container = TasksBootstrapper.Init();

                var cancellationToken = new CancellationTokenSource();

                Console.WriteLine("Starting to listen ...");

                using(var queue = container.Resolve<IMessageQueue>())
                {
                    await queue.SubscribeAsync("sync", container.Resolve<SyncTask>().ExecuteAsync, cancellationToken.Token);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}