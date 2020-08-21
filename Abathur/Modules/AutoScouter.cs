using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abathur.Constants;
using Abathur.Core;
using Abathur.Model;
using Abathur.Modules.Services;
using NydusNetwork.API.Protocol;
using NydusNetwork.Logging;

namespace Abathur.Modules
{
    class AutoScouter :IModule
    {
        private IIntelManager _intel;
        private ICombatManager _combat;
        private IUnit _scout;
        private ILogger _logger;
        private Point2D _eStart;
        private bool _scouting;

        public AutoScouter(IIntelManager intel, ICombatManager combat, ILogger logger)
        {
            _intel = intel;
            _combat = combat;
            _logger = logger;
        }
        public void Initialize()
        {
            
        }

        public void OnStart()
        {
            _scouting = false;
            _eStart = _intel.Colonies.First(c => c.IsStartingLocation).Point;
            _scout = _intel.WorkersSelf().FirstOrDefault();
            if (_scout == null) {_logger.LogWarning("No Scout available. AutoScouter doing nothing."); return;}
            
            _intel.Handler.RegisterHandler(Case.WorkerDestroyed, u => {
                if (u.Tag==_scout.Tag)
                {
                    _scout = _intel.WorkersSelf().FirstOrDefault(sc => sc.Tag != _scout.Tag);
                    if(_scout == null) { _logger.LogWarning("No Scout available. AutoScouter doing nothing."); return; }
                    _scouting = false;
                }
            });
        }

        public void OnStep()
        {
            if (_scouting)
            {
                foreach (var unit in _intel.UnitsEnemyVisible.Where(u => u.UnitType!= BlizzardConstants.Unit.Overlord || u.UnitType != BlizzardConstants.Unit.Overseer || u.UnitType != BlizzardConstants.Unit.OverlordTransport))
                {
                    if (MathServices.EuclidianDistance(unit,_scout)<10)
                    {
                        _combat.Move(_scout.Tag,GetFleePoint(_scout.Point, unit.Point));
                        _scouting = false;
                    }
                }
            }
            if (!_scouting)
            {
                var sortedColonies = _intel.Colonies.OrderBy(c => MathServices.EuclidianDistance(c.Point, _eStart));
                foreach (var colony in sortedColonies)
                {
                    _combat.Move(_scout.Tag,colony.Point, true);
                }
                _scouting = true;
            }
            if (_scout.Orders.Count==0)
            {
                _scouting = false;
            }
            
        }

        public void OnGameEnded()
        {
            
        }

        public void OnRestart()
        {
            
        }

        public Point2D GetFleePoint(Point2D fleer, Point2D threat)
        {
            var vector = new Point2D{ X = fleer.X - threat.X, Y = fleer.Y - threat.Y};
            var length = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            var normalizedX = vector.X / length;
            var normalizedY = vector.Y / length;
            return new Point2D {X = ((float)normalizedX*10) + fleer.X, Y = ((float)normalizedY*10)+fleer.Y};
        }
    }
}
