using System;
using Unity.Properties;
using UnityEngine;

[Serializable]
public class NavigationWaypoint : ScriptableObject
{
    private NavigationWaypoint[] next;
    
    public NavigationWaypoint[] Next { get => next ; set => next = value; }

    public Vector3 position;
    
    public void SetNext(NavigationWaypoint[] nextWaypoints)
    {
        next = nextWaypoints; 
    }

    public void SetNext(NavigationWaypoint nextWaypoints)
    {
        next = new[] { nextWaypoints }; 
    }
}
