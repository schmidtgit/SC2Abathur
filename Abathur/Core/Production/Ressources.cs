using Abathur.Core.Production;
using NydusNetwork.API.Protocol;

namespace Abathur.Model {
    internal class Ressources {
        public float Minerals { get; set; }
        public float Vespene { get; set; }
        public float Supply { get; set; }
        public Ressources() { }
        public Ressources(PlayerCommon common) {
            Minerals = common.Minerals;
            Vespene = common.Vespene;
            Supply = common.FoodCap - common.FoodUsed;
        }
        public static Ressources operator -(Ressources ressources,ProductionOrder order)
            => order.Unit == null ? ressources - order.Research : ressources - order.Unit;
        public static Ressources operator +(Ressources ressources,ProductionOrder order)
            => order.Unit == null ? ressources + order.Research : ressources + order.Unit;
        public static Ressources operator -(Ressources ressources,UnitTypeData unit) {
            if(unit == null) return ressources;
            return new Ressources {
                Minerals = ressources.Minerals - unit.MineralCost,
                Vespene = ressources.Vespene - unit.VespeneCost,
                Supply = ressources.Supply - unit.FoodRequired
            };
        }
        public static Ressources operator +(Ressources ressources,UnitTypeData unit) {
            if(unit == null) return ressources;
            return new Ressources {
                Minerals = ressources.Minerals + unit.MineralCost,
                Vespene = ressources.Vespene + unit.VespeneCost,
                Supply = ressources.Supply + unit.FoodRequired
            };
        }
        public static Ressources operator -(Ressources ressources,UpgradeData upgrade) {
            if(upgrade == null) return ressources;
            return new Ressources {
                Minerals = ressources.Minerals - upgrade.MineralCost,
                Vespene = ressources.Vespene - upgrade.VespeneCost
            };
        }
        public static Ressources operator +(Ressources ressources,UpgradeData upgrade) {
            if(upgrade == null) return ressources;
            return new Ressources {
                Minerals = ressources.Minerals + upgrade.MineralCost,
                Vespene = ressources.Vespene + upgrade.VespeneCost
            };
        }
        public static Ressources operator -(Ressources r1,Ressources r2) {
            return new Ressources {
                Minerals = r1.Minerals - r2.Minerals,
                Vespene = r1.Vespene - r2.Vespene,
                Supply = r1.Supply - r2.Supply
            };
        }
        public static Ressources operator +(Ressources r1,Ressources r2) {
            return new Ressources {
                Minerals = r1.Minerals + r2.Minerals,
                Vespene = r1.Vespene + r2.Vespene,
                Supply = r1.Supply + r2.Supply
            };
        }
    }
}
