namespace NydusNetwork.Logging
{
    public class MultiLogger : ILogger {
        private ILogger[] _logs;
        public MultiLogger(params ILogger[] logs) => _logs = logs;

        void ILogger.LogError(object o) {
            foreach(var log in _logs)
                log.LogError(o);
        }

        void ILogger.LogInfo(object o) {
            foreach(var log in _logs)
                log.LogInfo(o);
        }

        void ILogger.LogMessage(object o) {
            foreach(var log in _logs)
                log.LogMessage(o);
        }

        void ILogger.LogSuccess(object o) {
            foreach(var log in _logs)
                log.LogSuccess(o);
        }

        void ILogger.LogWarning(object o) {
            foreach(var log in _logs)
                log.LogWarning(o);
        }
    }
}
