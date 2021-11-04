using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDBReader.src
{
    public enum LogLevel { Debug, Release }
    public class Logger
    {
        public LogLevel level;

        public void Log (String toLog)
        {
            Console.WriteLine(toLog);
        }

        public void Log(String toLog, LogLevel level)
        {
            if (level < this.level)
            { 
                
            }
        }
    }
}
