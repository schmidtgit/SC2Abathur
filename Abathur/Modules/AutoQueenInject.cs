using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abathur.Constants;
using Abathur.Core;
using Abathur.Core.Combat;
using Abathur.Modules.Services;
using Abathur.Extensions;
using Abathur.Repositories;
using NydusNetwork.API.Protocol;
using NydusNetwork.Services;

namespace Abathur.Modules
{
    public class AutoQueenInject : IModule
    {
        private IIntelManager _intel;
        private ISquadRepository _squadRepo;
        private ulong _queenSquadId;
        private ulong _hatcheriesId;
        private ICombatManager _combatManager;

        public AutoQueenInject(IIntelManager intel, ISquadRepository squadRepo, ICombatManager combatManager)
        {
            _intel = intel;
            _squadRepo = squadRepo;
            _combatManager = combatManager;
        }
        public void Initialize()
        {
            
        }

        public void OnStart()
        {
            if(GameConstants.ParticipantRace != Race.Zerg)
                return;
            _queenSquadId = _squadRepo.Create("Queens").Id;
            var hatcheries = _squadRepo.Create("Hatcheries");
            hatcheries.AddUnit(_intel.StructuresSelf().First(u => u.UnitType == BlizzardConstants.Unit.Hatchery));
            _hatcheriesId = hatcheries.Id;

            _intel.Handler.RegisterHandler(Case.UnitAddedSelf, u =>
            {
                if (u.UnitType == BlizzardConstants.Unit.Queen)
                {
                    _squadRepo.Get().First(g => g.Name.Equals("Queens")).Units.Add(u);
                }
            });

            _intel.Handler.RegisterHandler(Case.StructureAddedSelf,u => {
                if(u.UnitType == BlizzardConstants.Unit.Hatchery || u.UnitType == BlizzardConstants.Unit.Lair || u.UnitType == BlizzardConstants.Unit.Hive) {
                    _squadRepo.Get().First(g => g.Name.Equals("Hatcheries")).Units.Add(u);
                }
            });
        }

        public void OnStep()
        {
            if (GameConstants.ParticipantRace != Race.Zerg)
                return;
            var queens = _squadRepo.Get(_queenSquadId);
            var hatcheries = _squadRepo.Get(_hatcheriesId).Units;
            if (queens.Units.Count!=0 && hatcheries.Count!= 0)
            {
                foreach(var hatchery in hatcheries) {
                    if(!hatchery.BuffIds.Contains(BlizzardConstants.Buffs.QueenSpawnLarvaTimer)) {
                        var queen = hatchery.GetClosest(queens.Units);
                        _combatManager.UseTargetedAbility(BlizzardConstants.Ability.SpawnLarva,queen.Tag,hatchery.Tag);
                    }
                }
            }
            
        }

        public void OnGameEnded()
        {
            
        }

        public void OnRestart()
        {
            
        }
    }
}
