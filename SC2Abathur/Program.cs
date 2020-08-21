using SC2Abathur.Settings;
using Abathur;
using NydusNetwork;
using System;
using System.IO;
using System.Reflection;
using NydusNetwork.Model;
using NydusNetwork.Logging;
using NydusNetwork.Services;
using SC2Abathur.Services;
using SC2Abathur.Client;

namespace Launcher {
    /// <summary>
    /// Default class for handling initial control of flow.
    /// </summary>
    class Program {
        /// <summary>
        /// Current version of the launcher - used for support.
        /// </summary>
        public static string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// Current version of the Abathur framework - used for support.
        /// </summary>
        public static string AbathurVersion { get; } = Assembly.GetAssembly(typeof(IAbathur)).GetName().Version.ToString();

        /// <summary>
        /// Current version of the NydusNetwork (C# alternative for the SC2api) - used for support.
        /// </summary>
        public static string NydusNetworkVersion { get; } = Assembly.GetAssembly(typeof(IGameClient)).GetName().Version.ToString();

        
        /// <summary>
        /// Entry point for the executable.
        /// Absoloute paths for data directory and log directory can be parsed as arguments.
        /// </summary>
        /// <param name="args">First argument will be used as datapath, second as logpath</param>
        static void Main(string[] args) {
            Console.Title = $"Abathur Framework v{AbathurVersion} and NydusNetwork v{NydusNetworkVersion}";
            var dataPath = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory() + "\\data\\";
            var logPath = args.Length > 1 ? args[0] : Directory.GetCurrentDirectory() + "\\log\\";
            new Program().Run(dataPath, logPath);
        }

        /// <summary>
        /// Run the Abathur framework with the settings specified in the data directory.
        /// </summary>
        /// <param name="dataPath">The absolute path for a directory where Abathur may place setting files</param>
        /// <param name="logPath">The absolute directory path for log files, no log files will be generated if left empty</param>
        public void Run(string dataPath,string logPath = null) {
            // ConsoleLogger prints in pretty colors...
            ILogger log = ConsoleLogger.Instance;

            // Check that the directory exist, or create it.
            log?.LogMessage("Checking data directory:");
            FileService.ValidateOrCreateDirectory(dataPath,log);

            // Check that the game settings file exist, or create it a default template.
            log?.LogMessage("Checking game settings file:");
            FileService.ValidateOrCreateJsonFile(dataPath + "gamesettings.json",Defaults.GameSettings,log);
            GameSettings gameSettings = FileService.ReadFromJson<GameSettings>(dataPath + "gamesettings.json",log);

            // Load the Abathur setup file, this file is used to add/remove modules to the framework
            log?.LogMessage("Checking setup file:");
            FileService.ValidateOrCreateJsonFile(dataPath + "setup.json",Defaults.AbathurSetup,log);
            AbathurSetup abathurSetup = FileService.ReadFromJson<AbathurSetup>(dataPath + "setup.json",log);

            // Load or create the 'essence file' - a file containing UnitTypeData, AbilityTypeData, BuffData, UpgradeData and manually coded tech-trees for each race.
            var essence = EssenceService.LoadOrCreate(dataPath,log);

            // If a log path have been specified, check the directory and change add a filelogger (writes to files)
            if(logPath != null) {
                log?.LogMessage("Checking log directory:");
                FileService.ValidateOrCreateDirectory(logPath,log);
                log = new MultiLogger(ConsoleLogger.Instance,new FileLogger(logPath,"abathur"));
            }

            // Check if gamesettings specify more than one participant (1 = Participant, 2 = Computer, 3 = Observer (not implemented))
            IClient player1 = null;
            IClient player2 = null;
            player1 = new AbathurClient(abathurSetup,gameSettings,essence,log);
            if(gameSettings.IsMultiplayer())
                player2 = new AbathurClient(abathurSetup,gameSettings,essence,log);

            // Start the game already!
            var game = new Game(gameSettings,player1,player2);
            game.ExecuteMatch();
        }
    }
}
