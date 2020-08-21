using System.Collections.Generic;

namespace SC2Abathur.Settings {
    /// <summary>
    /// Model class used to allow Abathur to be configured without recompiling.
    /// </summary>
    public class AbathurSetup {
        /// <summary>
        /// Toggle wheather Abathur should run all IModules in parallel (only gamestep).
        /// </summary>
        public bool IsParallelized { get; set; }

        /// <summary>
        /// Valid names of IModules or external commands for lanching Python clients.
        /// </summary>
        public List<string> Modules { get; set; } = new List<string>();
    }
}
