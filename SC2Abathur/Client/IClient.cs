namespace SC2Abathur.Client {
    /// <summary>
    /// Each IClient is responsible for launching their own instance of the StarCraft II client.
    /// </summary>
    public interface IClient {
        /// <summary>
        /// Launch a client and make everything ready for a CreateGame call.
        /// </summary>
        void Initialize();
        /// <summary>
        /// Create game should ONLY be called on the host!
        /// </summary>
        void CreateGame();
        /// <summary>
        /// Join game must be called on ALL participating clients - and only after the host have called CreateGame.
        /// </summary>
        void JoinGame();
        /// <summary>
        /// Tell the client wheather or not it should be the host.
        /// This should be called prior to calling create game.
        /// </summary>
        /// <param name="host"></param>
        void SetHost(bool host);
    }
}
