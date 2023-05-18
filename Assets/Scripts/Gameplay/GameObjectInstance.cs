using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectInstance
{
    static int lastAssignedID = 0;
    public int ID;

    public GameObjectInstance() { 
        ID = lastAssignedID++;
    }
}
