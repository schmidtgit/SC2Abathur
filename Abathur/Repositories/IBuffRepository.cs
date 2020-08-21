using System.Collections.Generic;
using NydusNetwork.API.Protocol;

namespace Abathur.Repositories {
    public interface IBuffRepository {
        BuffData Get(uint id);
        IEnumerable<BuffData> Get();
    }
}
