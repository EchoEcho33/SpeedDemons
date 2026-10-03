using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NavigationNetwork : MonoBehaviour
{
    public NavigationWaypoint head { get ; private set ;}
    
    private List<NavigationWaypoint> checkpointWaypoints = new();

    [SerializeField] 
    private TextAsset raceTelemetryJSON;

    public void Start()
    {
        ClearNetwork();
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
        
        int checkpointCounter = 0;
        List<NavigationWaypoint> markedWaypoints = new List<NavigationWaypoint> { curr };
        
        for (int i = 1; i < recording.frames.Count; i++)
        {
            RacerTelemetryFrame frame = recording.frames[i];
            NavigationWaypoint next = CreateWaypoint(recording.frames[i]);
            curr.SetNext(next);
            curr = next;
            
            if (checkpointCounter != frame.CheckpointID)
            {
                checkpointCounter++;
                markedWaypoints.Add(curr);
            }
        }
        
        curr.SetNext(head);
        
        for (int i = 0; i < markedWaypoints.Count; i++)
        {
            NavigationWaypoint startFinishLineWaypoint = CreateCheckpointWaypoint(i, markedWaypoints[i].Next);
            if (startFinishLineWaypoint != null) checkpointWaypoints.Add(startFinishLineWaypoint);
        }
    }
    
    private NavigationWaypoint CreateWaypoint(RacerTelemetryFrame frame)
    {
        NavigationWaypoint waypoint = ScriptableObject.CreateInstance<NavigationWaypoint>();
        waypoint.position = frame.RacerPosition;
        return waypoint;
    }
    
    private NavigationWaypoint CreateCheckpointWaypoint(int checkpointID, NavigationWaypoint[] nextWaypoints)
    {
        if (nextWaypoints == null) return null;
        
        NavigationWaypoint waypoint = ScriptableObject.CreateInstance<NavigationWaypoint>();
        TrackCheckpoint checkpoint = RaceManager.Instance.GetCheckpoint(checkpointID);
        if (checkpoint == null) return null;
        
        waypoint.position = checkpoint.GetPosition();
        waypoint.SetNext(nextWaypoints);
        return waypoint;
    }
    
    public NavigationWaypoint GetCheckpointWaypoint(int checkpointID)
    {
        if (checkpointID < 0 || checkpointWaypoints.Count <= checkpointID) return null;
        
        return checkpointWaypoints[checkpointID];
    }

    private void ClearNetwork()
    {
        if (head != null)
        {
            HashSet<NavigationWaypoint> visited = new();
            ClearNetworkHelper(head, visited);
        }
        
        foreach (NavigationWaypoint waypoint in checkpointWaypoints)
        {
            if (waypoint == null) continue;

            if (Application.isPlaying)
            {
                Destroy(waypoint);
            }
            else
            {
                DestroyImmediate(waypoint);
            }
        }
        
        checkpointWaypoints.Clear();
        head = null;
    }

    private void ClearNetworkHelper(NavigationWaypoint curr, HashSet<NavigationWaypoint> visited)
    {
        if (curr == null || !visited.Add(curr)) return;

        if (curr.Next != null)
        {
            foreach (NavigationWaypoint nextWaypoint in curr.Next)
            {
                ClearNetworkHelper(nextWaypoint, visited);
            }
        }

        if (Application.isPlaying)
        {
            Destroy(curr);
        }
        else
        {
            DestroyImmediate(curr);
        }
    }
    
#if UNITY_EDITOR
    [ContextMenu("Regenerate Network")]
    private void RegenerateNetwork()
    {
        ClearNetwork();
        GenerateNetwork();
    }

    private void OnValidate()
    {
        if (raceTelemetryJSON == null)
        {
            ClearNetwork();
        }
    }

    private void OnEnable()
    {
        ClearNetwork();
        GenerateNetwork();
    }

    private void OnDisable()
    {
        ClearNetwork();
    }
    
    private void OnDrawGizmos()
    {
        if (Application.isPlaying || head == null) return;
        
        Gizmos.color = new Color(0f, .5f, 1f, 0.5f);
        DrawWaypoint(head);
        
        foreach (NavigationWaypoint waypoint in checkpointWaypoints)
        {
            Gizmos.DrawSphere(waypoint.position, 1f);
            
            if (waypoint.Next == null) continue;
            foreach (NavigationWaypoint nextWaypoint in waypoint.Next)
            {
                Gizmos.DrawLine(waypoint.position, nextWaypoint.position);
            }
        }
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
            if (next == head) continue;
            DrawWaypoint(next);
        }
    }
#endif
}
