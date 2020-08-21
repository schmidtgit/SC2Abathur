using System;
using System.IO;
using System.Text;

namespace NydusNetwork.Logging {
    public class FileLogger : ILogger {
        private string _path;
        public FileLogger(string outputfolder, string name = "log") {
            _path = $"{outputfolder}\\{name}_{DateTime.Now.Ticks}.txt";
            using(FileStream fs = File.Create(_path)) {
                byte[] title = new UTF8Encoding(true).GetBytes($"| BEGINNING OF LOG - {DateTime.Now} |");
                fs.Write(title,0,title.Length);
            }
            WriteToFile("");
        }

        void ILogger.LogError(object s) => WriteToFile($"{DateTime.Now.TimeOfDay} |  ERROR  |\t {s}");

        void ILogger.LogInfo(object s) => WriteToFile($"{DateTime.Now.TimeOfDay}\t {s} (info)");

        void ILogger.LogMessage(object s) => WriteToFile($"{DateTime.Now.TimeOfDay}\t {s}");

        void ILogger.LogSuccess(object s) => WriteToFile($"{DateTime.Now.TimeOfDay} | SUCCESS |\t {s}");

        void ILogger.LogWarning(object s) => WriteToFile($"{DateTime.Now.TimeOfDay} | WARNING |\t {s}");

        public void WriteToFile(string s) {
            using(var file = new StreamWriter(_path,true))
                file.WriteLine(s);
        }
    }
}
