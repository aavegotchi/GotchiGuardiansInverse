using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialUIButtonData_Tower : RadialUIButtonData
{
    public TowerTemplate.TowerTypeID TypeID;

    public RadialUIButtonData_Tower(TowerTemplate.TowerTypeID typeID,  Texture buttonImage)
        : base(buttonImage)
    {
        TypeID = typeID;
    }
}
