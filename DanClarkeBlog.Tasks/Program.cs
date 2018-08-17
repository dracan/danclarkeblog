using System;
using System.Threading;
using Autofac;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Tasks.Tasks;

namespace DanClarkeBlog.Tasks
{
    internal static class Program
    {
        static void Main()
        {
            try
            {
                var container = TasksBootstrapper.Init();

                Console.WriteLine("Starting to listen ...");

                using(var queue = container.Resolve<IMessageQueue>())
                {
                    queue.Subscribe("sync", container.Resolve<SyncTask>().ExecuteAsync);

                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}