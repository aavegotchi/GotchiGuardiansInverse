using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    #region Fields
    [Header("Required Refs")]
    public static Transform[] points = null;
    #endregion

    #region Unity Functions
    void Awake()
    {
        points = new Transform[transform.childCount];
        for (int i=0; i<points.Length; i++)
        {
            points[i] = transform.GetChild(i);
        }
    }
    #endregion
}
