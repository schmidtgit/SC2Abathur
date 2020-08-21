namespace NydusNetwork.Logging {
    public interface ILogger {
        void LogMessage(object s);
        void LogSuccess(object s);
        void LogWarning(object s);
        void LogError(object s);
        void LogInfo(object s);
    }
}
