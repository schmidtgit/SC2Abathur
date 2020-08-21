using System.Collections.Generic;
using NydusNetwork.API.Protocol;

namespace Abathur.Repositories {
    public interface IUnitTypeRepository {
        UnitTypeData Get(uint id);
        IEnumerable<UnitTypeData> Get();
    }
}
