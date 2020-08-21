using NydusNetwork.API.Protocol;

namespace Abathur.Core {
    public interface IRawManager {
        /// <summary>
        /// Game status received on last response from StarCraft II client.
        /// </summary>
        Status Status { get; }
        /// <summary>
        /// Send request to StarCraft II client immediately (async).
        /// </summary>
        /// <param name="request">Request to send</param>
        void SendRawRequest(Request request);

        /// <summary>
        /// Send request to StarCraft II client immediately.
        /// </summary>
        /// <param name="req">Request to send</param>
        /// <param name="response">Response received</param>
        /// <returns>False if request timed out</returns>
        bool TryWaitRawRequest(Request req,out Response response);

        /// <summary>
        /// Send request to StarCraft II client immediately.
        /// </summary>
        /// <param name="req">Request to send</param>
        /// <param name="response">Response received</param>
        /// <param name="timeout">Set timeout value in ms</param>
        /// <returns>False if request timed out</returns>
        bool TryWaitRawRequest(Request req,out Response response,int timeout);

        /// <summary>
        /// Send observation request and wait for the response.
        /// Will keep asking until the gameloop have changed.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="gameloop"></param>
        /// <returns>False if request timed out</returns>
        bool TryWaitObservationRequest(out Response response,uint gameloop);

        /// <summary>
        /// Queue actions to send at the end of current game loop.
        /// </summary>
        /// <param name="actions">Actions to queue</param>
        void QueueActions(params Action[] actions);

        /// <summary>
        /// Realtime (faster gamespeed) or step-mode
        /// </summary>
        bool Realtime { get; set; }

        /// <summary>
        /// True if hosting - do not change mid-game.
        /// </summary>
        bool IsHosting { get; set; } 

        /// <summary>
        /// Send step request.
        /// </summary>
        void Step();

        /// <summary>
        /// Launch StarCraft II client and connect.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Create game using current game settings.
        /// </summary>
        /// <returns>Returns false if request failed</returns>
        bool CreateGame();

        /// <summary>
        /// Leave game.
        /// </summary>
        /// <returns>Returns false if request failed</returns>
        bool LeaveGame();

        /// <summary>
        /// Join game as race specified in game settings.
        /// Race can be changed between matches.
        /// </summary>
        /// <returns>Returns false if join request failed</returns>
        bool JoinGame();

        /// <summary>
        /// Will leave game, create game (if host) and attempt to join.
        /// </summary>
        void Restart();
    }
}
