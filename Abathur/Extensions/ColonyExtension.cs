using Abathur.Model;
using System.Linq;

namespace Abathur.Extensions
{
    public static class ColonyExtension
    {
        public static int OptimalVespene(this IColony colony) => colony.Vespene.Count() * 3;
        public static int OptimalMineral(this IColony colony) => colony.Minerals.Count() * 2;
    }
}
