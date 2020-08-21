using System.Collections.Generic;

namespace Abathur.Modules
{
    public interface IModule
    {
        void Initialize();
        /// <summary>
        /// handles what must happen when a Starcraft 2 game starts. 
        /// </summary>
        void OnStart();
        /// <summary>
        /// called everytime a step is taken in a SC2 game. In step mode this will be equivalent of an actual step.
        /// In real time mode a step is taken everytime the intel manager has been updated and all modules has taken 
        /// an action or forfeited their action for the step.
        /// </summary>
        void OnStep();
        /// <summary>
        /// handles what must happen when a Starcraft 2 game ends
        /// </summary>
        void OnGameEnded();
        /// <summary>
        /// Handles what must happen when a Starcraft 2 game restarts
        /// </summary>
        void OnRestart();
    }
}
