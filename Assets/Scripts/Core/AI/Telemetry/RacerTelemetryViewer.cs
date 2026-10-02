using UnityEngine;

public class RacerTelemetryViewer : MonoBehaviour
{ 
    [SerializeField] 
    private TextAsset raceTelemetryJSON;
    
    private RacerTelemetryRecording recording;

    private void OnValidate()
    {
        LoadRecording();
    }

    private void LoadRecording()
    {
        if (raceTelemetryJSON == null)
        {
            recording = null;
            return;
        }
        
        recording = JsonUtility.FromJson<RacerTelemetryRecording>(raceTelemetryJSON.text);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying || recording?.frames == null) return;
        
        Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
        for (int i = 1; i < recording.frames.Count; i++)
        {
            Vector3 previous = recording.frames[i - 1].RacerPosition;
            Vector3 current = recording.frames[i].RacerPosition;
            Gizmos.DrawLine(previous, current);
            Gizmos.DrawSphere(previous, 1f);
        }
    }
}
