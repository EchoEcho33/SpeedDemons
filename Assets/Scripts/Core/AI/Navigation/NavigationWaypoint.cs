using System;
using UnityEngine;

[Serializable]
public class NavigationWaypoint : ScriptableObject
{
    private NavigationWaypoint[] next;
    
    public NavigationWaypoint[] Next { get => next ; set => next = value; }

    public Vector3 position;

    /** Index of the racing line this waypoint belongs to. */
    public int lineIndex;

    /** ID of the track checkpoint segment this waypoint is in. */
    public int checkpointID;
    
    public void SetNext(NavigationWaypoint[] nextWaypoints)
    {
        next = nextWaypoints; 
    }

    public void SetNext(NavigationWaypoint nextWaypoints)
    {
        next = new[] { nextWaypoints };
    }

    public void AddNext(NavigationWaypoint nextWaypoint)
    {
        if (next == null)
        {
            SetNext(nextWaypoint);
            return;
        }

        NavigationWaypoint[] newNext = new NavigationWaypoint[next.Length + 1];
        next.CopyTo(newNext, 0);
        newNext[^1] = nextWaypoint;
        next = newNext;
    }
}
