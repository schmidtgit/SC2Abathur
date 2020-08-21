using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abathur.Constants;
using Abathur.Core;
using Abathur.Core.Combat;
using Abathur.Model;
using Abathur.Repositories;
using NydusNetwork.API.Protocol;

namespace Abathur.Modules
{
    class AutoPush :IModule
    {
        private IIntelManager _intel;
        private ICombatManager _combat;
        private ISquadRepository _squadRepository;
        private Point2D _eStart;
        private Squad _theGang;
        private bool _done;
        private int _autoPushArmySize;

        private readonly ISet<uint> _nonAttackUnits = new HashSet<uint>
        {
            BlizzardConstants.Unit.Overlord,
            BlizzardConstants.Unit.OverlordTransport,
            BlizzardConstants.Unit.HighTemplar,
            BlizzardConstants.Unit.Infestor,
            BlizzardConstants.Unit.Disruptor,
            BlizzardConstants.Unit.Oracle,
            BlizzardConstants.Unit.WarpPrism,
            BlizzardConstants.Unit.MULE,
            BlizzardConstants.Unit.WidowMine,
            BlizzardConstants.Unit.Larva,
            BlizzardConstants.Unit.SwarmHost,
        };

        public AutoPush(IIntelManager intel, ICombatManager combat, ISquadRepository squadRepository)
        {
            _intel = intel;
            _combat = combat;
            _squadRepository = squadRepository;
        }
        public void Initialize()
        {
            
        }

        public void OnStart()
        {
            _autoPushArmySize = 10;
            _eStart = _intel.Colonies.First(c => c.IsStartingLocation).Point;
            _theGang = _squadRepository.Create("TheGang");
            _intel.Handler.RegisterHandler(Case.UnitAddedSelf,HandleUnitMade);
        }

        public void OnStep()
        {
            if(_theGang.Units.Count >= _autoPushArmySize && !_done) {
                _combat.AttackMove(_theGang,_eStart);
                _done = true;
                _intel.Handler.DeregisterHandler(HandleUnitMadeAttack);
                _intel.Handler.RegisterHandler(Case.UnitAddedSelf,HandleUnitMadeAttack);
            }

            if(_theGang.Units.Count < _autoPushArmySize && _done) {
                _intel.Handler.DeregisterHandler(HandleUnitMadeAttack);
                _intel.Handler.RegisterHandler(Case.UnitAddedSelf,HandleUnitMade);
                _done = false;
            }
        }

        public void OnGameEnded()
        {
            
        }

        public void OnRestart()
        {
            
        }
        public void HandleUnitMadeAttack(IUnit u) {
            if(!(_nonAttackUnits.Contains(u.UnitType) || GameConstants.IsCocoon(u.UnitType))) {
                _theGang.AddUnit(u);
                _combat.AttackMove(_theGang,_eStart);
            }
        }

        public void HandleUnitMade(IUnit u) {
            if(!(_nonAttackUnits.Contains(u.UnitType) || GameConstants.IsCocoon(u.UnitType))) {
                _theGang.AddUnit(u);
            }
        }
    }
}
