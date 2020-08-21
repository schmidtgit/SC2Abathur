using System;
namespace NydusNetwork.Logging {
    public class ConsoleLogger : ILogger {
        private static ConsoleLogger _log;
        public ConsoleLogger() {}
        public static ConsoleLogger Instance {
            get {
                if(_log != null)
                    return _log;
                return _log = new ConsoleLogger();
            }
        }

        void ILogger.LogMessage(object s)   => Console.WriteLine(s);

        void ILogger.LogError(object s)     => ColoredMessage(s,ConsoleColor.Red);

        void ILogger.LogInfo(object s)      => ColoredMessage(s,ConsoleColor.Blue);

        void ILogger.LogSuccess(object s)   => ColoredMessage(s,ConsoleColor.Green);

        void ILogger.LogWarning(object s)   => ColoredMessage(s,ConsoleColor.Yellow);

        private void ColoredMessage(object s, ConsoleColor c) {
            var standard = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.WriteLine(s);
            Console.ForegroundColor = standard;
        }
    }
}
