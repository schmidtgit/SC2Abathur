using Abathur.Repositories;
using NydusNetwork.API.Protocol;
using NydusNetwork.Logging;
using System.IO;
using Abathur.Constants;

namespace Abathur.Modules
{
    class GenerateFile : IModule {
        IUnitTypeRepository _unitRepo;
        public GenerateFile(IUnitTypeRepository unitRepo) {
            _unitRepo = unitRepo;
        }
        void IModule.OnGameEnded() { }
        void IModule.Initialize() { }
        void IModule.OnStart() {
            var path = Directory.GetCurrentDirectory() + "\\log\\";
            var file = new FileLogger(path,"generated");


            foreach(var unitTypeData in _unitRepo.Get()) {
                if(!GameConstants.IsMorphed(unitTypeData.UnitId))
                    continue;

                file.WriteToFile("/// <summary>");
                file.WriteToFile($"/// MineralCost for {unitTypeData.Name}");
                file.WriteToFile("/// </summary>");
                file.WriteToFile($"public const int {unitTypeData.Name.Replace(" ","")} = {unitTypeData.MineralCost};");
                file.WriteToFile($"public const int {unitTypeData.Name.Replace(" ","")} = {unitTypeData.VespeneCost};");
            }

        }
        void IModule.OnStep() {}
        void IModule.OnRestart() {}
    }
}