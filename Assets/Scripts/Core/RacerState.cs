using TMPro.EditorUtilities;
using UnityEngine;

public class RacerState : MonoBehaviour
{
    public TrackCheckpoint currCheckpoint { get; private set;}

    //Lap Counter for all racers
    public int currLap { get; private set; }
    public StartFinishCheckpoint startFinishCheckpoint;

    public void StartRace(StartFinishCheckpoint startFinishCheckpoint)
    {
        currCheckpoint = startFinishCheckpoint;
        this.startFinishCheckpoint = startFinishCheckpoint;
        currLap = 1;
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        if (currCheckpoint == null) return;

        BoxCollider box = currCheckpoint.nextCheckpoint.box;
        if (box == null) return;
        
        Gizmos.matrix = box.transform.localToWorldMatrix;

        if (currCheckpoint.nextCheckpoint is StartFinishCheckpoint nextCheckpoint)
        {
            Gizmos.color = new Color(0f, 1f, 1f, 0.25f);
            Gizmos.DrawCube(box.center, box.size);
        
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(box.center, box.size);
        }
        else
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
            Gizmos.DrawCube(box.center, box.size);
        
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(box.center, box.size);
        }
    }
#endif

    public void ReachCheckpoint(TrackCheckpoint checkpoint)
    {
        currCheckpoint = checkpoint;
        if (checkpoint == startFinishCheckpoint) { currLap++; 
            FindFirstObjectByType<RaceManager>().playerLaps.text = "Lap " + currLap + "/3"; }
        }
}
