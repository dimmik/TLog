using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLog
{
    public enum LogLevel : int
    {
        Trace = 30, Debug = 20, Info = 10, Warn = 5, Error = 0, FatalError = -10
    }
}
