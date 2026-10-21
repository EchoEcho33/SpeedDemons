using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
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

    [SerializeField]
    private List<RacingLineSource> racingLines = new();

    [SerializeField, Range(0f, 90f), Tooltip("Largest turn, in degrees, a racer has to make to switch onto another racing line.")]
    private float maxLinkAngle = 25f;

    [SerializeField, Min(0f), Tooltip("Longest a link between racing lines can be.")]
    private float maxLinkDistance = 30f;

    // Single racing line fields from before racing lines were added. Scenes saved with these still load as line 0.
    [SerializeField, HideInInspector]
    private TextAsset raceTelemetryJSON;

    [SerializeField, HideInInspector]
    private TextAsset navNetworkJSON;

    private readonly List<RacingLine> lines = new();

    private readonly List<RacingLineSource> lineSources = new();    // the source each entry in lines was built from

    public int RacingLineCount => lines.Count;

    /** Callback for when a navigation network is generated. */
    public event Action OnNetworkGenerated;

    public void Start()
    {
        ClearNetwork();
        GenerateNetwork();
    }

    private void GenerateNetwork()
    {
#if UNITY_EDITOR
        generatedSources = GetSourceFiles();
#endif
        foreach (RacingLineSource source in GetLineSources())
        {
            if (source == null || !source.HasData) continue;

            RacingLine line = RacingLine.Build(lines.Count, source.LoadWaypointData(), GetCheckpointPosition);
            if (line.Head == null) continue;

            lines.Add(line);
            lineSources.Add(source);
        }

        if (lines.Count == 0) return;

        RebuildLinks();
        OnNetworkGenerated?.Invoke();
        Debug.LogError("Navigation Network Generated. AI Initialized.");
    }

    /// <summary>
    /// Recalculates the links between racing lines, e.g. after waypoints have been moved.
    /// </summary>
    public void RebuildLinks()
    {
        RacingLineLinker.Link(lines, maxLinkAngle, maxLinkDistance);
    }

    private List<RacingLineSource> GetLineSources()
    {
        if (racingLines.Count > 0) return racingLines;

        List<RacingLineSource> legacySources = new();
        if (raceTelemetryJSON != null || navNetworkJSON != null)
        {
            legacySources.Add(new RacingLineSource { raceTelemetryJSON = raceTelemetryJSON, navNetworkJSON = navNetworkJSON });
        }
        return legacySources;
    }

    private static Vector3? GetCheckpointPosition(int checkpointID)
    {
        if (RaceManager.Instance == null) return null;

        TrackCheckpoint checkpoint = RaceManager.Instance.GetCheckpoint(checkpointID);
        return checkpoint != null ? checkpoint.GetPosition() : null;
    }

    public NavigationWaypoint GetLineHead(int lineIndex)
    {
        if (lineIndex < 0 || lines.Count <= lineIndex) return null;

        return lines[lineIndex].Head;
    }

    public NavigationWaypoint GetCheckpointWaypoint(int lineIndex, int checkpointID)
    {
        if (lineIndex < 0 || lines.Count <= lineIndex) return null;

        return lines[lineIndex].GetCheckpointWaypoint(checkpointID);
    }

    private void ClearNetwork()
    {
        foreach (RacingLine line in lines)
        {
            line.Destroy();
        }

        lines.Clear();
        lineSources.Clear();
#if UNITY_EDITOR
        editedLines.Clear();
#endif
    }

#if UNITY_EDITOR
    private static readonly Color[] LineColors =
    {
        new(0f, .5f, 1f, 0.5f),
        new(1f, .5f, 0f, 0.5f),
        new(.6f, 0f, 1f, 0.5f),
        new(0f, 1f, .5f, 0.5f),
    };

    private static readonly Color LinkColor = new(1f, 1f, 1f, 0.35f);

    private List<TextAsset> generatedSources = new();     // the files the current racing lines were generated from

    private readonly HashSet<int> editedLines = new();      // racing lines with moved waypoints that haven't been saved yet

    private List<TextAsset> GetSourceFiles()
    {
        List<TextAsset> files = new();
        foreach (RacingLineSource source in GetLineSources())
        {
            files.Add(source?.raceTelemetryJSON);
            files.Add(source?.navNetworkJSON);
        }
        return files;
    }

    [ContextMenu("Regenerate Network")]
    private void RegenerateNetwork()
    {
        ClearNetwork();
        GenerateNetwork();
    }

    private void OnValidate()
    {
        // Move the single racing line fields into the racing lines list so they show up in the inspector.
        if (racingLines.Count == 0 && (raceTelemetryJSON != null || navNetworkJSON != null))
        {
            racingLines.Add(new RacingLineSource { raceTelemetryJSON = raceTelemetryJSON, navNetworkJSON = navNetworkJSON });
            raceTelemetryJSON = null;
            navNetworkJSON = null;
        }

        // If only the link settings changed, just relink so unsaved waypoint edits aren't thrown away.
        if (GetSourceFiles().SequenceEqual(generatedSources))
        {
            RebuildLinks();
            SceneView.RepaintAll();
            return;
        }

        // Regenerating destroys waypoints, which isn't allowed during OnValidate, so wait until after it.
        EditorApplication.delayCall -= RegenerateAfterValidate;
        EditorApplication.delayCall += RegenerateAfterValidate;
    }

    private void RegenerateAfterValidate()
    {
        if (this == null || !isActiveAndEnabled || Application.isPlaying) return;

        RegenerateNetwork();
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
        if (Application.isPlaying) return;

        foreach (RacingLine line in lines)
        {
            Gizmos.color = LineColors[line.Index % LineColors.Length];

            foreach (NavigationWaypoint waypoint in line.Waypoints)
            {
                Gizmos.DrawSphere(waypoint.position, 1f);
                if (waypoint.Next?[0] != null) Gizmos.DrawLine(waypoint.position, waypoint.Next[0].position);
            }

            foreach (NavigationWaypoint waypoint in line.CheckpointWaypoints)
            {
                Gizmos.DrawSphere(waypoint.position, 1f);
                if (waypoint.Next?[0] != null) Gizmos.DrawLine(waypoint.position, waypoint.Next[0].position);
            }
        }

        // Links to other racing lines are everything after Next[0].
        Gizmos.color = LinkColor;
        foreach (RacingLine line in lines)
        {
            foreach (NavigationWaypoint waypoint in line.Waypoints)
            {
                if (waypoint.Next == null) continue;

                for (int i = 1; i < waypoint.Next.Length; i++)
                {
                    if (waypoint.Next[i] != null) Gizmos.DrawLine(waypoint.position, waypoint.Next[i].position);
                }
            }
        }
    }

    public List<NavigationWaypoint> GetNetworkWaypoints()
    {
        List<NavigationWaypoint> waypoints = new();
        foreach (RacingLine line in lines)
        {
            waypoints.AddRange(line.Waypoints);
        }
        return waypoints;
    }

    public void MarkLineEdited(int lineIndex)
    {
        editedLines.Add(lineIndex);
    }

    /// <summary>
    /// Saves each racing line with moved waypoints to its nav network JSON. Links between racing lines aren't saved,
    /// they're rebuilt from waypoint positions on load, so lines that weren't edited don't need saving.
    /// </summary>
    [ContextMenu("Save Changes to Nav Network JSON")]
    public void SaveNetworkChanges()
    {
        if (editedLines.Count == 0)
        {
            Debug.Log("No racing lines have been edited, nothing to save.");
            return;
        }

        foreach (int lineIndex in editedLines.ToList())
        {
            if (SaveLine(lines[lineIndex], lineSources[lineIndex])) editedLines.Remove(lineIndex);
        }

        // Saving can assign new nav network JSONs, which shouldn't count as a source change that regenerates the network.
        generatedSources = GetSourceFiles();
        EditorUtility.SetDirty(this);
    }

    private bool SaveLine(RacingLine line, RacingLineSource source)
    {
        string relativePath;
        if (source.navNetworkJSON != null)
        {
            relativePath = AssetDatabase.GetAssetPath(source.navNetworkJSON).Remove(0, 7); // remove /Assets/
        }
        else
        {
            relativePath = $"{SceneManager.GetActiveScene().name}_NavNetwork_Line{line.Index}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json";
        }

        string path = Path.Combine(Application.dataPath, relativePath); // save it to current nav network JSON path
        try
        {
            var navNetworkData = new NavigationNetworkData(line.ToWaypointData());

            string json = JsonUtility.ToJson(navNetworkData);
            File.WriteAllText(path, json);
            AssetDatabase.ImportAsset("Assets/" + relativePath);
            AssetDatabase.Refresh();
            source.navNetworkJSON = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/" + relativePath);

            Debug.Log($"Racing line {line.Index} saved to: {path}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }
#endif
}
