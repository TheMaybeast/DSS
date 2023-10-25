using System;
using System.IO;
using System.Reflection;

namespace DSS.Utils
{
    class Log
    {
        public Log()
        {
            string message = "DSS - Dynamic Siren System v" + Assembly.GetExecutingAssembly().GetName().Version;
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            string path = @"Plugins/DSS.log";
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }
    }
}
