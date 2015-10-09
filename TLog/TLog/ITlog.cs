using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TLog
{
    public interface ITlog
    {
        void Trace(string msg);
        void Debug(string msg);
        void Info(string msg);
        void Warn(string msg);

        void Error(string msg, Exception e);

        void Log(LogLevel level, string msg, Exception e);

        void Flush();
    }
}
