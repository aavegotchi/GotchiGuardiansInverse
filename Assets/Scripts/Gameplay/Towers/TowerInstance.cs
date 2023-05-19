using Fusion;
using UnityEngine;
using static TowerInstance;

public class TowerInstance : GameObjectInstance
{
    public enum State
    {
        New, // Initial state, should switch to Building automatically in controller
        Building, // Represents the tower being built
        Idle, // Represents the tower being idle, likely searching for its victim. Purely passive towers remain in this state
        Acting, // Represents the tower doing logic to launch its attack
        Cooldown, // Represents the tower waiting for its cooldown to finish before returning to idle (after attacking / using ability)
        Upgrading, // Represents an upgrade in progress
    };

    #region Variables
    [SerializeField]
    public TowerController Controller { get; set; }

    public delegate void StateChanged(TowerInstance instance, State state);
    public event StateChanged OnStateChanged;

    private State _currentState = State.New;
    public State CurrentState 
    {
        get { return _currentState; }
        private set
        {
            if (_currentState != value)
            {
                _currentState = value;
                OnStateChanged?.Invoke(this, _currentState);
            }
        } 
    }

    [SerializeField]
    public TowerTemplate Template { get; set; } = null;
    public TowerPedastalInstance Pedastal { get; set; } = null;
    #endregion

    #region functions
    void Start()
    {
        gameObject.name = "Tower Instance [" + Template.name + "]: " + ID;
    }
    #endregion
}
