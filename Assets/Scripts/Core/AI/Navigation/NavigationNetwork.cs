using System;
using System.Collections.Generic;
using UnityEngine;

public class NavigationNetwork : MonoBehaviour
{
    public NavigationWaypoint head { get ; private set ;}

    [SerializeField] 
    private TextAsset raceTelemetryJSON;

    public void Start()
    {
        GenerateNetwork();
    }
    
    // TODO: Change this to be more than just a network from a recording from telemetry :D
    private void GenerateNetwork()
    {
        if (raceTelemetryJSON == null)
        {
            return;
        }
        
        RacerTelemetryRecording recording = JsonUtility.FromJson<RacerTelemetryRecording>(raceTelemetryJSON.text);
        NavigationWaypoint curr = CreateWaypoint(recording.frames[0]);
        head = curr;
        
        for (int i = 1; i < recording.frames.Count; i++)
        {
            NavigationWaypoint next = CreateWaypoint(recording.frames[i]);
            curr.SetNext(next);
            curr = next;
        }
    }
    
    private NavigationWaypoint CreateWaypoint(RacerTelemetryFrame frame)
    {
        NavigationWaypoint waypoint = ScriptableObject.CreateInstance<NavigationWaypoint>();
        waypoint.position = frame.RacerPosition;
        return waypoint;
    }
    
#if UNITY_EDITOR
    
    public void OnValidate()
    {
        GenerateNetwork();
    }
    
    private void OnDrawGizmos()
    {
        if (Application.isPlaying || head == null) return;
        
        Gizmos.color = new Color(0f, .5f, 1f, 0.5f);
        DrawWaypoint(head);
    }

    private void DrawWaypoint(NavigationWaypoint curr)
    {
        if (curr == null) return;
        Vector3 currPos = curr.position;
        Gizmos.DrawSphere(currPos, 1f);
        
        if (curr.Next == null) return;
        
        foreach (NavigationWaypoint next in curr.Next)
        {
            if (next == null) continue;
            
            Vector3 nextPos = next.position;
            Gizmos.DrawLine(currPos, nextPos);
            DrawWaypoint(next);
        }
    }
#endif
}
