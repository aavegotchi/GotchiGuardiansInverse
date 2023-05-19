
using Fusion;

public class GameObjectInstance : NetworkBehaviour
{
    static int lastAssignedID = 0;

    public int ID;

    public GameObjectInstance() { 
    }

    private void Awake()
    {
        ID = lastAssignedID++;
    }
}
