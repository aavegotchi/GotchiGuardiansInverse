using System;
using PhaseManager;
using Gotchi.Lickquidator.Manager;

namespace GameMaster
{
    public class GameMasterEvents
    {
        public static MenuEventsBlueprint MenuEvents = new MenuEventsBlueprint();
        public static PhaseEventsBlueprint PhaseEvents = new PhaseEventsBlueprint();
        public static TowerEventsBlueprint TowerEvents = new TowerEventsBlueprint();
        public static EnemyEventsBlueprint EnemyEvents = new EnemyEventsBlueprint();
        public static GotchiEventsBlueprint GotchiEvents = new GotchiEventsBlueprint();
        public static PoolEventsBlueprint PoolEvents = new PoolEventsBlueprint();
    }

    public class MenuEventsBlueprint
    {
        public Action MenuItemSelectedLong = delegate { };
        public Action MenuItemSelectedShort = delegate { };
    }

    public class PhaseEventsBlueprint
    {
        public Action<Phase> PhaseChanged = delegate { };
        public Action MainMenuStarted = delegate { };
        public Action PrepPhaseStarted = delegate { };
        public Action SurvivalPhaseStarted = delegate { };
        public Action<Phase> TransitionPhaseStarted = delegate { };
    }

    public class TowerEventsBlueprint
    {
        public Action TowerStarted = delegate { };
        public Action<TowerBlueprint> TowerFinished = delegate { };
        public Action<TowerPool.TowerType> TowerAttacked = delegate { };
        public Action<TowerPool.TowerType> TowerHit = delegate { };
        public Action<TowerPool.TowerType> TowerDied = delegate { };
    }

    public class EnemyEventsBlueprint
    {
        public Action EnemyStarted = delegate { };
        public Action<EnemyBlueprint> EnemyFinished = delegate { };
        public Action<LickquidatorManager.LickquidatorType> EnemyAttacked = delegate { };
        public Action<LickquidatorManager.LickquidatorType> EnemyHit = delegate { }; // currently unused
        public Action<LickquidatorManager.LickquidatorType> EnemyDied = delegate { };
    }

    public class GotchiEventsBlueprint
    {
        public Action<int, GotchiManager.AttackType> GotchiAttacked = delegate { };
        public Action<int, int> GotchiDamaged = delegate { };
        public Action<int> GotchiHit = delegate { }; // currently unused
        public Action<int> GotchiDied = delegate { };
        public Action GotchisAllDead = delegate { };
    }

    public class PoolEventsBlueprint
    {
        public Action AudioPoolReady = delegate { };
        public Action HealthBarPoolReady = delegate { };
    }

    public class SettingsBlueprint
    {

    }
}
