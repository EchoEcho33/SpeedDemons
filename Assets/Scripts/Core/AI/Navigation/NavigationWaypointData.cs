using System;
using UnityEngine;

[Serializable]
public struct NavigationWaypointData
{
    public Vector3 Position;

    public int CheckpointID;

    public NavigationWaypointData(Vector3 position, int checkpointID)
    {
        Position = position;
        CheckpointID = checkpointID;
    }
}
