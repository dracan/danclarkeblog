using DanClarkeBlog.Core.Helpers;
using Microsoft.Azure.WebJobs.Host;

namespace DanClarkeBlog.Functions
{
    public class TraceLogLoggerImpl : ILogger
    {
        private readonly TraceWriter _traceWriter;

        public TraceLogLoggerImpl(TraceWriter traceWriter)
        {
            _traceWriter = traceWriter;
        }

        public void Trace(string msg)
        {
            _traceWriter.Verbose(msg);
        }

        public void Info(string msg)
        {
            _traceWriter.Info(msg);
        }

        public void Debug(string msg)
        {
            _traceWriter.Verbose(msg);
        }

        public void Error(string msg)
        {
            _traceWriter.Error(msg);
        }
    }
}