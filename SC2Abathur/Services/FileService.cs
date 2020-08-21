using Newtonsoft.Json;
using NydusNetwork.Logging;
using System;
using System.IO;

namespace SC2Abathur.Services {

    static class FileService {
        /// Read from path deserialize to T
        public static T ReadFromJson<T>(string path,ILogger log = null) {
            if(File.Exists(path)) {
                log?.LogSuccess($"\tLOADED: {path}");
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            }
            log?.LogError($"\tCOULD NOT FIND: {path}");
            return default(T);
        }

        // Check if it exist and create it if not.
        public static void ValidateOrCreateDirectory(string path,ILogger log = null) {
            if(Directory.Exists(path)) {
                log?.LogSuccess($"\tFOUND: {path}");
            } else {
                Directory.CreateDirectory(path);
                log?.LogWarning($"\tCREATED: {path}");
            }
        }

        // Check if file exist and deserialize content to path if it does not.
        public static void ValidateOrCreateJsonFile(string path,object content,ILogger log = null) {
            try {
                if(File.Exists(path)) {
                    log?.LogSuccess($"\tFOUND: {path}");
                } else {
                    File.WriteAllText(path,JsonConvert.SerializeObject(content,Formatting.Indented));
                    log?.LogWarning($"\tCREATED: {path}");
                }
            } catch(Exception e) { log?.LogError($"\tFAILED: {e.Message}"); }
        }
    }
}
