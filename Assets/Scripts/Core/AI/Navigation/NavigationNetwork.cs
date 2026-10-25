using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class NavigationNetwork : MonoBehaviour
{
    [Serializable]
    public class NavigationNetworkData      // used for saving and loading waypoint data from navNetworkJSON
    {
        public List<NavigationWaypointData> waypoints;

        public NavigationNetworkData(List<NavigationWaypointData> waypoints)
        {
            this.waypoints = waypoints;
        }
    }

    public NavigationWaypoint head { get ; private set ;}
    
    private List<NavigationWaypoint> checkpointWaypoints = new();

#if UNITY_EDITOR
    private List<NavigationWaypoint> networkWaypoints = new();  // used for tracking all waypoints for editor
#endif

    [SerializeField] 
    private TextAsset raceTelemetryJSON;

    [SerializeField] 
    private TextAsset navNetworkJSON;

    /** Callback for when a navigation network is generated. */
    public event Action OnNetworkGenerated;
    
    public void Start()
    {
        ClearNetwork();
        GenerateNetwork();
    }
    
    // TODO: Change this to be more than just a network from a recording from telemetry :D
    private void GenerateNetwork()
    {
        if (navNetworkJSON != null)
        {
            GenerateNetworkFromNavNetworkJSON();
        } 
        else if (raceTelemetryJSON != null)
        {
            GenerateNetworkFromTelemetryJSON();
        }
    }

    private void GenerateNetworkFromNavNetworkJSON() 
    {
        NavigationNetworkData navNet = JsonUtility.FromJson<NavigationNetworkData>(navNetworkJSON.text);
        NavigationWaypoint curr = CreateWaypoint(navNet.waypoints[0].Position);
        head = curr;

        int checkpointCounter = 0;
        List<NavigationWaypoint> markedWaypoints = new List<NavigationWaypoint> { curr };

        for (int i = 1; i < navNet.waypoints.Count; i++)
        {
            NavigationWaypointData waypointData = navNet.waypoints[i];
            NavigationWaypoint next = CreateWaypoint(navNet.waypoints[i].Position);
            curr.SetNext(next);
            curr = next;

            if (checkpointCounter != waypointData.CheckpointID)
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

    private void GenerateNetworkFromTelemetryJSON()
    {
        RacerTelemetryRecording recording = JsonUtility.FromJson<RacerTelemetryRecording>(raceTelemetryJSON.text);
        NavigationWaypoint curr = CreateWaypoint(recording.frames[0].RacerPosition);
        head = curr;

        int checkpointCounter = 0;
        List<NavigationWaypoint> markedWaypoints = new List<NavigationWaypoint> { curr };

        for (int i = 1; i < recording.frames.Count; i++)
        {
            RacerTelemetryFrame frame = recording.frames[i];
            NavigationWaypoint next = CreateWaypoint(recording.frames[i].RacerPosition);
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
        
        OnNetworkGenerated?.Invoke();
        Debug.LogError("Navigation Network Generated. AI Initialized.");
    }

    private NavigationWaypoint CreateWaypoint(Vector3 position)
    {
        NavigationWaypoint waypoint = ScriptableObject.CreateInstance<NavigationWaypoint>();
        waypoint.position = position;

#if UNITY_EDITOR
        networkWaypoints.Add(waypoint);
#endif
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

#if UNITY_EDITOR
        networkWaypoints.Clear();
#endif
        
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

    public List<NavigationWaypoint> GetNetworkWaypoints()
    {
        return networkWaypoints;
    }

    [ContextMenu("Save Changes to Nav Network JSON")]
    public void SaveNetworkChanges()
    {
        if (head == null) return;

        List<NavigationWaypointData> waypoints = new();
        List<NavigationWaypoint> afterCheckPointWaypoints = new();

        foreach (var checkpoint in checkpointWaypoints)
        {
            if (checkpoint.Next != null)
            {
                foreach (var waypoint in checkpoint.Next)
                {
                    afterCheckPointWaypoints.Add(waypoint);
                }
            }
        }

        PopulateWaypointData(afterCheckPointWaypoints, waypoints, head, 0);

        string relativePath;
        if (navNetworkJSON != null)
        {
            relativePath = AssetDatabase.GetAssetPath(navNetworkJSON).Remove(0, 7); // remove /Assets/
        } 
        else
        {
            relativePath = $"{SceneManager.GetActiveScene().name}_NavNetwork_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json";
        }

        string path = Path.Combine(Application.dataPath, relativePath); // save it to current nav network JSON path 
        try
        {
            var navNetworkData = new NavigationNetworkData(waypoints);
            
            string json = JsonUtility.ToJson(navNetworkData);
            File.WriteAllText(path, json);
            AssetDatabase.ImportAsset("Assets/" + relativePath);
            AssetDatabase.Refresh();
            navNetworkJSON = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/" + relativePath);

            Debug.Log($"Navigation network saved to: {path}");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void PopulateWaypointData(List<NavigationWaypoint> afterCheckPointWaypoints, List<NavigationWaypointData> waypoints, NavigationWaypoint curr, int currCheckpoint)
    {
        if (curr == null) return;

        if (!checkpointWaypoints.Contains(curr)) 
        {
            if (curr != head && curr.Next != null && curr.Next[0] != null)
            {
                if (afterCheckPointWaypoints.Contains(curr.Next[0]))
                {
                    currCheckpoint++;

                }
            }

            var data = new NavigationWaypointData(curr.position, currCheckpoint);   // need to get checkpoint ID from somewhere
            waypoints.Add(data);
        }

        if (curr.Next == null) return;

        foreach (NavigationWaypoint next in curr.Next)
        {
            if (next == null) continue;
            if (next == head) continue;

            PopulateWaypointData(afterCheckPointWaypoints, waypoints, next, currCheckpoint);
        }
    }
#endif
}
