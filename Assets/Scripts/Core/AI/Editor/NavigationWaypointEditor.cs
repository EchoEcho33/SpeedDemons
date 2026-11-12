using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[CustomEditor(typeof(NavigationNetwork))]
public class NavigationWaypointEditor : Editor
{
    HashSet<NavigationWaypoint> selectedWaypoints = new();

    Vector2 startDrag = Vector2.zero;

    bool dragging = false;
    bool deselecting = false;

    private void OnSceneGUI()
    {
        NavigationNetwork navNetwork = (NavigationNetwork)target;

        List<NavigationWaypoint> waypoints = navNetwork.GetNetworkWaypoints();

        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.keyCode == KeyCode.Mouse0)
        {
            startDrag = e.mousePosition;
        }
        else if (e.type == EventType.MouseUp && e.keyCode == KeyCode.Mouse0 && dragging)
        {
            Vector2 endDrag = e.mousePosition;

            dragging = false;

            Event.current.Use();
        }
        else if (e.type == EventType.MouseDrag && e.keyCode == KeyCode.Mouse0 && (e.shift || (e.command || e.control)))
        {
            dragging = true;
            deselecting = !e.shift;
            Event.current.Use();
        }

        foreach (NavigationWaypoint waypoint in waypoints)
        {
            var radius = selectedWaypoints.Contains(waypoint) ? 1 : 0;

            if (Handles.Button(waypoint.position, Quaternion.identity, radius, 2, Handles.SphereHandleCap)) 
            {
                if (e.shift)
                {
                    if (selectedWaypoints.Contains(waypoint))
                    {
                        selectedWaypoints.Remove(waypoint);
                    } else
                    {
                        selectedWaypoints.Add(waypoint);
                    }
                } else
                {
                    selectedWaypoints.Clear();
                    selectedWaypoints.Add(waypoint);
                }
                
            }
        }

        if (selectedWaypoints.Count > 0)
        {
            Vector3 combinedPos = Vector3.zero;

            foreach (var w in selectedWaypoints) 
            {
                combinedPos += w.position;
            }

            Vector3 averagePos = combinedPos / selectedWaypoints.Count;

            EditorGUI.BeginChangeCheck();
            Vector3 newPosition = Handles.PositionHandle(averagePos, Quaternion.identity); // spawns a position change handle

            Vector3 deltaPos = (newPosition - averagePos);

            if (EditorGUI.EndChangeCheck())
            {
                foreach (var w in selectedWaypoints)
                {
                    Undo.RecordObject(w, "Change position"); // this is so undo and redo works with these changes
                    w.position = w.position + deltaPos;
                }
            }
        }

        if (dragging)
        {
            Handles.BeginGUI();

            Vector2 endDrag = e.mousePosition;

            float minX = Mathf.Min(startDrag.x, endDrag.x);
            float minY = Mathf.Min(startDrag.y, endDrag.y);

            float maxX = Mathf.Max(startDrag.x, endDrag.x);
            float maxY = Mathf.Max(startDrag.y, endDrag.y);

            float w = maxX - minX;
            float h = maxY - minY;

            Rect r = new Rect(minX, minY, w, h);

            // Draw a GUI Box (Rectangle)
            GUI.Box(r, "");

            Handles.EndGUI();

            var foundWaypoints = FindWaypointsInBox(deselecting ? selectedWaypoints.ToList() : waypoints, minX, minY, maxX, maxY);
            foreach (var waypoint in foundWaypoints)
            {
                if (deselecting)
                {
                    selectedWaypoints.Remove(waypoint);
                } else
                {
                    selectedWaypoints.Add(waypoint);
                }
            }
        }
    }

    private List<NavigationWaypoint> FindWaypointsInBox(List<NavigationWaypoint> allWaypoints, float minX, float minY, float maxX, float maxY)
    {
        List<NavigationWaypoint> foundWaypoints = new List<NavigationWaypoint>();

        foreach (var waypoint in allWaypoints) 
        {
            var screenPos = HandleUtility.WorldToGUIPoint(waypoint.position);
            
            if (screenPos.x > minX && screenPos.x < maxX && screenPos.y > minY && screenPos.y < maxY)
            {
                foundWaypoints.Add(waypoint);
            }
        }


        return foundWaypoints;
    }
}
