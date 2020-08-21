using Abathur.Factory;
using Google.Protobuf;
using NydusNetwork.API.Protocol;
using NydusNetwork.Logging;
using System;
using System.IO;

namespace SC2Abathur.Services {

    /// <summary>
    /// Used to generate the 'essence' file.
    /// Essence contains information that may change between patches - but not between matches.
    /// </summary>
    public class EssenceService {
        /// <summary>
        /// Will load the essence file from the given path or attempt to fetch information from client and write to location.
        /// </summary>
        /// <param name="dataPath">Valid path for data directory used by Abathur</param>
        /// <param name="log">Optional log</param>
        /// <returns></returns>
        public static Essence LoadOrCreate(string dataPath, ILogger log = null) {
            log?.LogMessage("Checking binary essence file:");
            ValidateOrCreateBinaryFile(dataPath + "essence.data",() => FetchDataFromClient(log),log);
            return Load(dataPath + "essence.data",log);
        }

        /// <summary>
        /// Will load the essence file from the desired path (assumed stored as binary protobuf).
        /// </summary>
        /// <param name="path">Path to essence file</param>
        /// <param name="log">Optional log</param>
        /// <returns></returns>
        public static Essence Load(string path,ILogger log = null) {
            using(var stream = File.OpenRead(path)) {
                var result = Essence.Parser.ParseFrom(stream);
                log?.LogSuccess($"\tLOADED: {path}");
                return result;
            }
        }

        /// <summary>
        /// Will validate existence of file (not content) or attempt to write file.
        /// </summary>
        /// <typeparam name="T">Any object that is a protobuf message</typeparam>
        /// <param name="path">Path to validate</param>
        /// <param name="content">Function to get content in case the file does not exist</param>
        /// <param name="log">Optional log</param>
        private static void ValidateOrCreateBinaryFile<T>(string path,Func<T> content,ILogger log = null) where T : IMessage {
            try {
                if(File.Exists(path)) {
                    log?.LogSuccess($"\tFOUND: {path}");
                } else {
                    var msg = content.Invoke();
                    FileStream stream = File.Create(path);
                    msg.WriteTo(stream);
                    stream.Close();
                    log?.LogWarning($"\tCREATED: {path}");
                }
            } catch(Exception e) { log.LogError($"\tFAILED: {e.Message}"); }
        }

        /// <summary>
        /// Will launch a StarCraft II client and attempt to gather information using the DataRequest.
        /// </summary>
        /// <param name="log">Optional log</param>
        /// <returns></returns>
        private static Essence FetchDataFromClient(ILogger log = null) {
            var factory = new EssenceFactory(log);
            return factory.FetchFromClient(Settings.Defaults.GameSettings);
        }
    }
}
