using NydusNetwork.API.Protocol;
using System.Collections.Generic;

namespace Abathur.Repositories {
    public interface IAbilityRepository {
        AbilityData Get(uint id);
        IEnumerable<AbilityData> Get();
    }
}
