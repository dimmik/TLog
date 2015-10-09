using System;
using System.Threading.Tasks;

namespace TLog
{
    internal class AsyncTLog : ITlog
    {
        private readonly ITlog _log;

        public AsyncTLog(ITlog log)
        {
            _log = log;
        }

        public void Trace(string msg)
        {
            Task.Factory.StartNew(() => _log.Trace(msg));
        }

        public void Debug(string msg)
        {
            Task.Factory.StartNew(() => _log.Debug(msg));
        }

        public void Info(string msg)
        {
            Task.Factory.StartNew(() => _log.Info(msg));
        }

        public void Warn(string msg)
        {
            Task.Factory.StartNew(() => _log.Warn(msg));
        }

        public void Error(string msg, Exception e)
        {
            Task.Factory.StartNew(() => _log.Error(msg, e));
        }

        public void Log(LogLevel level, string msg, Exception e)
        {
            Task.Factory.StartNew(() => _log.Log(level, msg, e));
        }

        public void Flush()
        {
            Task.Factory.StartNew(_log.Flush);
        }
    }
}
