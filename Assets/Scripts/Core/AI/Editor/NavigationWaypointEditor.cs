using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NavigationNetwork))]
public class NavigationWaypointEditor : Editor
{
    NavigationWaypoint selectedWaypoint;

    private void OnEnable()
    {
        Undo.undoRedoPerformed += RebuildLinks;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= RebuildLinks;
    }

    // Links between racing lines depend on waypoint positions, so they're recalculated whenever a waypoint moves.
    private void RebuildLinks()
    {
        if (target == null) return;

        ((NavigationNetwork)target).RebuildLinks();
        SceneView.RepaintAll();
    }

    private void OnSceneGUI()
    {
        NavigationNetwork navNetwork = (NavigationNetwork)target;

        List<NavigationWaypoint> waypoints = navNetwork.GetNetworkWaypoints();

        foreach (NavigationWaypoint waypoint in waypoints)
        {
            if (Handles.Button(waypoint.position, Quaternion.identity, 0, 1, Handles.SphereHandleCap)) 
            {
                selectedWaypoint = waypoint;
            }
        }

        if (selectedWaypoint != null)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 newPosition = Handles.PositionHandle(selectedWaypoint.position, Quaternion.identity); // spawns a position change handle

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedWaypoint, "Change position"); // this is so undo and redo works with these changes
                selectedWaypoint.position = newPosition;
                navNetwork.MarkLineEdited(selectedWaypoint.lineIndex);
                navNetwork.RebuildLinks();
            }
        }
    }
}
