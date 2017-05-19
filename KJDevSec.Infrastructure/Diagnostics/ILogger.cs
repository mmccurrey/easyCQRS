using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KJDevSec.Diagnostics
{
    public interface ILogger
    {
        void Info(string message);
        void Info(string format, params object[] args);
        void Warning(string message);
        void Warning(string format, params object[] args);
        void Error(string message);
        void Error(string format, params object[] args);
    }
}
