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

    public RacerTelemetryFrame(float time, int lap, float steering, float throttle, float brake, Vector3 racerPosition)
    {
        Time = time;
        Lap = lap;
        Steering = steering;
        Throttle = throttle;
        Brake = brake;
        RacerPosition = racerPosition;
    }
}
