using System.IO;
using Microsoft.Azure.WebJobs;

namespace DanClarkeBlog.Tasks
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger("queue")] string message, TextWriter log)
        {
            log.WriteLine(message);
        }

        public static void ProcessTimer([TimerTrigger("*/5 * * * * *", RunOnStartup = true)] TimerInfo info, TextWriter log)
        {
            log.WriteLine("CALLING PROCESS TIMER");
        }
    }
}
