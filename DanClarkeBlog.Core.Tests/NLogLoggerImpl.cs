using NLog;
using ILogger = DanClarkeBlog.Core.Helpers.ILogger;

namespace DanClarkeBlog.Core.Tests
{
    class NLogLoggerImpl : ILogger
    {
        private readonly Logger _nlogLogger;

        public NLogLoggerImpl(Logger nlogLogger)
        {
            _nlogLogger = nlogLogger;
        }

        public void Trace(string msg)
        {
            _nlogLogger.Trace(msg);
        }

        public void Info(string msg)
        {
            _nlogLogger.Info(msg);
        }

        public void Debug(string msg)
        {
            _nlogLogger.Debug(msg);
        }

        public void Error(string msg)
        {
            _nlogLogger.Error(msg);
        }
    }
}
