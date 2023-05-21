
using Fusion;
using System;

public class GameObjectInstance : NetworkBehaviour
{
    static int lastAssignedID = 0;

    public int ID;

    public event Action<GameObjectInstance, bool> OnVisualsEnabledChanged = delegate { };

    private bool _visualsEnabled = true;
    public bool VisualsEnabled
    {
        get { return _visualsEnabled; }
        set
        {
            if (_visualsEnabled != value)
            {
                _visualsEnabled = value;
                VisualsEnabledChanged(value);
                OnVisualsEnabledChanged(this, value);
            }
        }
    }

    public GameObjectInstance() { 
    }

    private void Awake()
    {
        ID = lastAssignedID++;
    }

    protected virtual void VisualsEnabledChanged(bool enabled)
    {

    }
}
