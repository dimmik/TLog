using System;
using System.Collections.Generic;
using NLog;
using TLog;
using LogFactory = TLog.LogFactory;
using LogLevel = TLog.LogLevel;
using System.Threading;

namespace TLogConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = LogManager.GetLogger("console");
            log.Trace("trace");
            log.Debug("debug");
            log.Info("info");
            log.Warn("warn");
            log.Error("error");
            Console.WriteLine("press any key");
            Console.ReadKey();
            var tlogFactory = new LogFactory(minLevel: LogLevel.Info)
            {
                Trace = log.Trace,
                Debug = log.Debug,
                Info = m =>
                {
                    log.Info(m);
                    Thread.Sleep(1000);
                },
                Error = (msg, e) => log.Error(e, msg),
                Flush = (msg, e) => LogManager.GetLogger("file").Error(e, msg),

                //FlushLevel = LogLevel.Debug
            };
            //tlogFactory.Trace = null;
            TestLog(tlogFactory.GetLogger("test logger"));
            TestLog(tlogFactory.GetAsyncLogger("test async logger"));
            Console.WriteLine("press any key");
            Console.ReadKey();
        }

        private static void TestLog(ITlog tlog)
        {
            tlog.Trace("trace");
            tlog.Debug("debug");
            for (var i = 0; i < 20; i++)
            {
                tlog.Info("info: " + i);
            }
            try
            {
                MethodWitnIntentionalNullpointer();
            }
            catch (Exception e)
            {
                tlog.Error("error", e);
            }
        }

        private static void MethodWitnIntentionalNullpointer()
        {
            // intentional null pointer reference
            ((IList<string>) null).Clear();
        }
    }
}
