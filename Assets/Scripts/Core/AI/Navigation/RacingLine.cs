using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The recorded files a racing line is built from. A nav network JSON (hand edited waypoints) takes priority over a telemetry recording.
/// </summary>
[Serializable]
public class RacingLineSource
{
    public TextAsset raceTelemetryJSON;

    public TextAsset navNetworkJSON;

    public bool HasData => navNetworkJSON != null || raceTelemetryJSON != null;

    public List<NavigationWaypointData> LoadWaypointData()
    {
        if (navNetworkJSON != null)
        {
            return JsonUtility.FromJson<NavigationNetwork.NavigationNetworkData>(navNetworkJSON.text)?.waypoints;
        }

        if (raceTelemetryJSON != null)
        {
            RacerTelemetryRecording recording = JsonUtility.FromJson<RacerTelemetryRecording>(raceTelemetryJSON.text);
            if (recording?.frames == null) return null;

            List<NavigationWaypointData> data = new(recording.frames.Count);
            foreach (RacerTelemetryFrame frame in recording.frames)
            {
                data.Add(new NavigationWaypointData(frame.RacerPosition, frame.CheckpointID));
            }
            return data;
        }

        return null;
    }
}

/// <summary>
/// A single looping racing line. Next[0] of every waypoint is always the next waypoint on this same line,
/// any links to other racing lines come after it.
/// </summary>
public class RacingLine
{
    public int Index { get; }

    public NavigationWaypoint Head => waypoints.Count > 0 ? waypoints[0] : null;

    public IReadOnlyList<NavigationWaypoint> Waypoints => waypoints;

    /** Waypoints placed at each track checkpoint, used to rejoin this line after respawning. Indexed by checkpoint ID. */
    public IReadOnlyList<NavigationWaypoint> CheckpointWaypoints => checkpointWaypoints;

    private readonly List<NavigationWaypoint> waypoints = new();

    private readonly List<NavigationWaypoint> checkpointWaypoints = new();

    private RacingLine(int index)
    {
        Index = index;
    }

    /// <summary>
    /// Builds a looping racing line from recorded waypoint data.
    /// </summary>
    /// <param name="getCheckpointPosition">Returns the world position of a track checkpoint by ID, or null if there is no such checkpoint.</param>
    public static RacingLine Build(int index, List<NavigationWaypointData> data, Func<int, Vector3?> getCheckpointPosition)
    {
        RacingLine line = new RacingLine(index);
        if (data == null || data.Count == 0) return line;

        int checkpointCounter = 0;
        List<NavigationWaypoint> markedWaypoints = new();

        for (int i = 0; i < data.Count; i++)
        {
            NavigationWaypoint waypoint = CreateWaypoint(data[i].Position, index, data[i].CheckpointID);
            if (i > 0) line.waypoints[i - 1].SetNext(waypoint);
            line.waypoints.Add(waypoint);

            // The first waypoint after each checkpoint crossing marks where that checkpoint's segment begins.
            if (i == 0)
            {
                markedWaypoints.Add(waypoint);
            }
            else if (checkpointCounter != data[i].CheckpointID)
            {
                checkpointCounter++;
                markedWaypoints.Add(waypoint);
            }
        }

        line.waypoints[^1].SetNext(line.Head);

        for (int i = 0; i < markedWaypoints.Count; i++)
        {
            Vector3? checkpointPosition = getCheckpointPosition(i);
            if (checkpointPosition == null) continue;

            NavigationWaypoint checkpointWaypoint = CreateWaypoint(checkpointPosition.Value, index, i);
            checkpointWaypoint.SetNext(markedWaypoints[i].Next[0]);
            line.checkpointWaypoints.Add(checkpointWaypoint);
        }

        return line;
    }

    /// <summary>
    /// Removes all links to other racing lines, leaving each waypoint linked only to the next waypoint on this line.
    /// </summary>
    public void ResetLinks()
    {
        for (int i = 0; i < waypoints.Count; i++)
        {
            waypoints[i].SetNext(waypoints[(i + 1) % waypoints.Count]);
        }
    }

    public NavigationWaypoint GetCheckpointWaypoint(int checkpointID)
    {
        if (checkpointID < 0 || checkpointWaypoints.Count <= checkpointID) return null;

        return checkpointWaypoints[checkpointID];
    }

    public List<NavigationWaypointData> ToWaypointData()
    {
        List<NavigationWaypointData> data = new(waypoints.Count);
        foreach (NavigationWaypoint waypoint in waypoints)
        {
            data.Add(new NavigationWaypointData(waypoint.position, waypoint.checkpointID));
        }
        return data;
    }

    public void Destroy()
    {
        foreach (NavigationWaypoint waypoint in waypoints) DestroyWaypoint(waypoint);
        foreach (NavigationWaypoint waypoint in checkpointWaypoints) DestroyWaypoint(waypoint);

        waypoints.Clear();
        checkpointWaypoints.Clear();
    }

    private static NavigationWaypoint CreateWaypoint(Vector3 position, int lineIndex, int checkpointID)
    {
        NavigationWaypoint waypoint = ScriptableObject.CreateInstance<NavigationWaypoint>();
        waypoint.position = position;
        waypoint.lineIndex = lineIndex;
        waypoint.checkpointID = checkpointID;
        return waypoint;
    }

    private static void DestroyWaypoint(NavigationWaypoint waypoint)
    {
        if (waypoint == null) return;

        if (Application.isPlaying)
        {
            UnityEngine.Object.Destroy(waypoint);
        }
        else
        {
            UnityEngine.Object.DestroyImmediate(waypoint);
        }
    }
}
