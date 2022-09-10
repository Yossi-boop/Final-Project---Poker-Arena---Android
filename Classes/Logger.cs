using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public static class Logger
    {
        public static string Path = "C:/Users/Yossi/Desktop/Final Project/BackLog.txt";
        private static readonly object postLock = new object();

        public static void WriteToLogger(string message)
        {
            lock (postLock)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Path, true))
                {
                    file.WriteLine(message);
                }
            }
        }
    }

    
}
