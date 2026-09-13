using UnityEditor;
using UnityEngine;

public class StartFinishCheckpoint : TrackCheckpoint
{
    
#if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        
        if (box == null) return;
        
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color(0f, 1f, 1f, 0.25f);
        Gizmos.DrawCube(box.center, box.size);
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(box.center, box.size);
        
        Handles.color = new Color(0f, 1f, 1f, 0.5f);
        Vector3 conePosition = transform.TransformPoint(box.center) + GetTrackForwardDirection() * Vector3.forward * box.size.x * 2;
        Handles.ConeHandleCap(0, conePosition, GetTrackForwardDirection(), 5.0f, EventType.Repaint);
    }
#endif
}
