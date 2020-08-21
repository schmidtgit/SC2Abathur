using System.Collections.Generic;
using NydusNetwork.API.Protocol;

namespace Abathur.Repositories {
    public interface IUpgradeRepository {
        UpgradeData Get(uint id);
        IEnumerable<UpgradeData> Get();
    }
}
