using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using TLog;

namespace TLogConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = NLog.LogManager.GetLogger("console");
            log.Trace("trace");
            log.Debug("debug");
            log.Info("info");
            log.Warn("warn");
            log.Error("error");
            Console.WriteLine("press any key");
            Console.ReadKey();
            var tlogFactory = new TLog.LogFactory(minLevel: LogLevel.Info)
            {
                Trace = log.Trace,
                Debug = log.Debug,
                Info = log.Info,
                Error = (msg, e) => log.Error(e, msg),
                Flush = (msg, e) => NLog.LogManager.GetLogger("file").Error(e, msg),

                //FlushLevel = LogLevel.Debug
            };
            //tlogFactory.Trace = null;
            var tlog = tlogFactory.GetLogger("test logger");
            tlog.Trace("trace");
            tlog.Debug("debug");
            tlog.Info("info");
            try
            {
                MethodWitnIntentionalNullpointer();
            }
            catch (Exception e)
            {
                tlog.Log(LogLevel.FatalError, "fatal error", e);
            }

            tlog.Trace("2 - trace");
            tlog.Debug("2 - debug");
            tlog.Info("2 - info");
            try
            {
                MethodWitnIntentionalNullpointer();
            }
            catch (Exception e)
            {
                tlog.Log(LogLevel.FatalError, "2 - fatal error", e);
            }
            Console.WriteLine("press any key");
            Console.ReadKey();
        }

        private static void MethodWitnIntentionalNullpointer()
        {
            // intentional null pointer reference
            ((IList<string>) null).Clear();
        }
    }
}
