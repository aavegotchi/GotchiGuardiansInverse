using Gotchi.New;

namespace Gotchi.Events
{
    public class EventBus
    {
        public delegate void EmptyFn();
        public delegate void TransitionPhaseFn(PhaseManager.Phase nextPhase);
        public delegate void TowerFn(TowerPool.TowerType towerType);
        public delegate void EnemyFn(LickquidatorManager.LickquidatorType enemyType);
        public delegate void GotchiFn(GotchiManager.AttackType attackType);
        public delegate void TowerBuildFn(TowerBlueprint towerBlueprint);
        public delegate void EnemyBuildFn(EnemyBlueprint enemyBlueprint);
        
        public static MenuEventsBlueprint MenuEvents = new MenuEventsBlueprint();
        public static PhaseEventsBlueprint PhaseEvents = new PhaseEventsBlueprint();
        public static TowerEventsBlueprint TowerEvents = new TowerEventsBlueprint();
        public static EnemyEventsBlueprint EnemyEvents = new EnemyEventsBlueprint();
        public static GotchiEventsBlueprint GotchiEvents = new GotchiEventsBlueprint();
        public static PoolEventsBlueprint PoolEvents = new PoolEventsBlueprint();
    }

    public class MenuEventsBlueprint
    {
        public EventBus.EmptyFn MenuItemSelectedLong;
        public EventBus.EmptyFn MenuItemSelectedShort;
    }

    public class PhaseEventsBlueprint
    {
        public EventBus.EmptyFn MainMenuStarted;
        public EventBus.EmptyFn PrepPhaseStarted;
        public EventBus.EmptyFn SurvivalPhaseStarted;
        public EventBus.TransitionPhaseFn TransitionPhaseStarted;
    }

    public class TowerEventsBlueprint
    {
        public EventBus.EmptyFn TowerStarted;
        public EventBus.TowerBuildFn TowerFinished;
        public EventBus.TowerFn TowerAttacked;
        public EventBus.TowerFn TowerHit;
        public EventBus.TowerFn TowerDied;
    }

    public class EnemyEventsBlueprint
    {
        public EventBus.EmptyFn EnemyStarted;
        public EventBus.EnemyBuildFn EnemyFinished;
        public EventBus.EnemyFn EnemyAttacked;
        public EventBus.EnemyFn EnemyHit; // currently unused
        public EventBus.EnemyFn EnemyDied;
    }

    public class GotchiEventsBlueprint
    {
        public EventBus.GotchiFn GotchiAttacked;
        public EventBus.EmptyFn GotchiHit; // currently unused
        public EventBus.EmptyFn GotchiDied;
    }

    public class PoolEventsBlueprint
    {
        public EventBus.EmptyFn AudioPoolReady;
        public EventBus.EmptyFn HealthBarPoolReady;
    }

    public class SettingsBlueprint
    {

    }
}
