using System.Collections.Generic;
using Abathur.Core.Combat;

namespace Abathur.Repositories
{
    public interface ISquadRepository
    {
        Squad Create(string name);
        Squad Create(string name, ulong id);
        Squad Get(ulong id);
        Squad Get(string name);
        IEnumerable<Squad> Get();
        bool Remove(ulong id);
        void Clear();
    }
}
