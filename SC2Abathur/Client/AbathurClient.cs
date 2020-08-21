using Abathur;
using Abathur.Factory;
using SC2Abathur.Settings;
using NydusNetwork.API.Protocol;
using NydusNetwork.Logging;
using NydusNetwork.Model;
using System;

namespace SC2Abathur.Client {
    /// <summary>
    /// Abathur client for setting up the Abathur framework
    /// </summary>
    class AbathurClient : IClient {
        private IAbathur abathur;
        private bool isHost;

        private AbathurSetup setupSettings;
        private GameSettings gameSettings;
        private Essence essence;
        private ILogger log;

        /// <summary>
        /// Abathur client is used to set up the Abathur framework.
        /// </summary>
        /// <param name="setupSettings">File containing list of modules Abathur should include</param>
        /// <param name="gameSettings">Settings for setting up the game with the StarCraft II client</param>
        /// <param name="essence">File containing data that is subject to change due to patches</param>
        /// <param name="log">Optional logging, will be used be the entire framework</param>
        public AbathurClient(AbathurSetup setupSettings,GameSettings gameSettings,Essence essence,ILogger log = null) {
            this.setupSettings = setupSettings;
            this.gameSettings = gameSettings;
            this.essence = essence;
            this.log = log;
        }

        /// <inheritdoc />
        public void Initialize() {
            var factory = new AbathurFactory(log);
            abathur = factory.Create(gameSettings,essence,log,this.GetType().Assembly, setupSettings.Modules.ToArray());
            abathur.IsHosting = isHost;
            abathur.Initialize();
        }

        /// <inheritdoc />
        public void SetHost(bool host) => isHost = host;

        /// <inheritdoc />
        public void CreateGame() => abathur.CreateGame();

        /// <inheritdoc />
        public void JoinGame() {
            if(abathur == null)
                throw new ArgumentNullException("Initialize AbathurClient before calling run!");
            abathur.Run();
        }
    }
}
