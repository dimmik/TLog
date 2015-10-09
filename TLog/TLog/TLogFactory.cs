using System;
using System.Collections.Generic;

namespace TLog
{
    public class LogFactory
    {
        #region Logging Delegates definition

        public Action<string> Trace
        {
            set { LogActions[LogLevel.Trace] = (msg, e) => value(msg); }
        }

        public Action<string> Debug
        {
            set { LogActions[LogLevel.Debug] = (msg, e) => value(msg); }
        }

        public Action<string> Info
        {
            set { LogActions[LogLevel.Info] = (msg, e) => value(msg); }
        }

        public Action<string> Warn
        {
            set { LogActions[LogLevel.Warn] = (msg, e) => value(msg); }
        }

        public Action<string, Exception> Error
        {
            set { LogActions[LogLevel.Error] = value; }
        }

        public Action<string, Exception> FatalError
        {
            set { LogActions[LogLevel.FatalError] = value; }
        }

        public readonly Dictionary<LogLevel, Action<string, Exception>> LogActions =
            new Dictionary<LogLevel, Action<string, Exception>>();

        #endregion

        public Action<string, Exception> Flush = (s, e) => { };

        public LogLevel FlushLevel = LogLevel.Error;


        internal void DoLog(LogLevel level, string msg, Exception e)
        {
            if (_minLevel < level) return;
            if (LogActions.ContainsKey(level)) // if not - just do nothing
            {
                LogActions[level](msg, e);
            }
        }

        public Func<LogLevel, DateTimeOffset, string, Exception, string> FormatFlush =
            (level, dt, msg, e) => "level: " + level + " ts: " + dt + " msg: " + msg + " ex: '" +
                                   (e == null ? "" : e.Message) +
                                   "' stack: '" +
                                   (e == null
                                       ? ""
                                       : e.StackTrace) + "'";

        public Func<string, DateTimeOffset, string> FormatFlushBegin =
            (id, dt) =>
                "------ BEGIN Error flush for '" + id + "' timestamp: " + dt.ToString("dd.MM.yyyy HH:mm:ss.ffffzzz");

        public Func<string, DateTimeOffset, string> FormatFlushEnd =
            (id, dt) =>
                "------ END Error flush for '" + id + "' timestamp: " + dt.ToString("dd.MM.yyyy HH:mm:ss.ffffzzz");

        private readonly LogLevel _minLevel;

        public LogFactory(LogLevel minLevel)
        {
            _minLevel = minLevel;
        }

        public ITlog GetLogger(string identifier, bool emptyOnFlush = true)
        {
            return new TransactionalLog(this, identifier)
            {
                EmptyMessagesOnFlush = emptyOnFlush
            };
        }
    }

    class TransactionalLog : ITlog
    {


        private readonly LogFactory _factory;
        private readonly string _identifier;
        public bool EmptyMessagesOnFlush = true;

        public TransactionalLog(LogFactory factory,
            string id)
        {
            _factory = factory;
            _identifier = id;
        }

        
        private readonly object _messageLock = new object();
        private readonly List<LogMessage> _messages = new List<LogMessage>(); 

        private void StoreMessage(LogLevel level, string msg, Exception e)
        {
            var lm = new LogMessage()
            {
                Time = DateTimeOffset.Now,
                Level = level,
                Message = msg,
                Exception = e
            };
            lock (_messageLock)
            {
                _messages.Add(lm);
            }
        }

        public void Flush()
        {
            var now = DateTimeOffset.Now;
            _factory.Flush(_factory.FormatFlushBegin(_identifier, now), null);
            lock (_messageLock)
            {
                foreach (var lm in _messages)
                {
                    _factory.Flush(_factory.FormatFlush(lm.Level, lm.Time, lm.Message, lm.Exception), lm.Exception);
                }
                if (EmptyMessagesOnFlush) 
                    _messages.Clear();
            }
            _factory.Flush(_factory.FormatFlushEnd(_identifier, now), null);
        }

        public void Log(LogLevel level, string msg, Exception e)
        {
            StoreMessage(level, msg, e);
            _factory.DoLog(level, msg, e);
            if (level <= _factory.FlushLevel)
                Flush();
        }

        public void Trace(string msg)
        {
            Log(LogLevel.Trace, msg, null);
        }

        public void Debug(string msg)
        {
            Log(LogLevel.Debug, msg, null);
        }

        public void Info(string msg)
        {
            Log(LogLevel.Info, msg, null);
        }

        public void Warn(string msg)
        {
            Log(LogLevel.Warn, msg, null);
        }

        public void Error(string msg, Exception e)
        {
            Log(LogLevel.Error, msg, e);
        }


    }

    class LogMessage
    {
        public LogLevel Level;
        public DateTimeOffset Time;
        public string Message;
        public Exception Exception;
    }
}
