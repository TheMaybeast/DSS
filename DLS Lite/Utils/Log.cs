using System;
using System.IO;
using System.Reflection;

namespace DLSLite.Utils
{
    class Log
    {
        public Log()
        {
            string message = "DLS Lite - Dynamic Lighting System - Lite Edition - v" + Assembly.GetExecutingAssembly().GetName().Version;
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            string path = @"Plugins/DLS Lite.log";
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }
    }
}
