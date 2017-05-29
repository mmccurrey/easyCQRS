using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Diagnostics
{
    class NullConsoleLogger : ILogger
    {
        public void Error(string message)
        {
            
        }

        public void Error(string format, params object[] args)
        {
            
        }

        public void Info(string message)
        {
            
        }

        public void Info(string format, params object[] args)
        {
            
        }

        public void Warning(string message)
        {
            
        }

        public void Warning(string format, params object[] args)
        {
            
        }
    }
}
