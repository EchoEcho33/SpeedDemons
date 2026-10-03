using System;
using UnityEngine;

[Serializable]
public struct RacerTelemetryFrame
{
    public float Time;
    
    public int Lap;
    
    public float Steering;
    
    public float Throttle;
    
    public float Brake;
    
    public Vector3 RacerPosition;

    public int CheckpointID;

    public RacerTelemetryFrame(float time, int lap, float steering, float throttle, float brake, Vector3 racerPosition, int checkpointID)
    {
        Time = time;
        Lap = lap;
        Steering = steering;
        Throttle = throttle;
        Brake = brake;
        RacerPosition = racerPosition;
        CheckpointID = checkpointID;
    }
}
