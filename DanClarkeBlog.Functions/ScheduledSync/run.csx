#r "DanClarkeBlog.Core"

using System;
using DanClarkeBlog.Core.Helpers;

public static void Run(TimerInfo myTimer, TraceWriter log)
{
    log.Info($"*** C# Timer trigger function executed at: {DateTime.Now}");    

    var helper = new BlogPostSummaryHelper();

    log.Info(helper.GetSummaryText("A-----B"));
}