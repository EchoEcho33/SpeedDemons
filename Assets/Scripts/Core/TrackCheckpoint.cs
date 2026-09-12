using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class TrackCheckpoint : MonoBehaviour
{
    public BoxCollider box;

    private Vector3 CheckpointCenter => transform.TransformPoint(box.center);

    public TrackCheckpoint prevCheckpoint;
    public TrackCheckpoint nextCheckpoint;
    
    private void Start()
    {
        UpdateBoxColliderPosition();
    }
    
#if UNITY_EDITOR
    private void OnEnable()
    {
        Undo.postprocessModifications += UpdateBoxCollider;
    }

    private void OnDisable()
    {
        Undo.postprocessModifications -= UpdateBoxCollider;
    }
    
    protected virtual void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        if (box == null) return;
        
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        Gizmos.DrawCube(box.center, box.size);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(box.center, box.size);
    }
    
    protected virtual void OnDrawGizmosSelected()
    {
        if (Application.isPlaying) return;
        
        if (prevCheckpoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(prevCheckpoint.CheckpointCenter, CheckpointCenter);
        }
        
        if (nextCheckpoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(CheckpointCenter, nextCheckpoint.CheckpointCenter);
        }   
    }

    private UndoPropertyModification[] UpdateBoxCollider(UndoPropertyModification[] undoPropertyModifications)
    {
        foreach (UndoPropertyModification modification in undoPropertyModifications)
        {
            if (modification.currentValue.target is Component component && component.gameObject == gameObject)
            {
                if (component == box) UpdateBoxColliderPosition();
                break;
            }
        }

        return undoPropertyModifications;
    }
#endif

    private void UpdateBoxColliderPosition()
    {
        box.center = Vector3.up * box.size.y * 0.5f;
    }

    private void OnTriggerEnter(Collider other)
    {
        Drive drive = other.gameObject.GetComponentInParent<Drive>();
        RacerController racer = drive.Racer;
        if (racer == null)
        {
            Debug.LogError("Racer not found.");
            return;
        }
        
        RacerState racerState = racer.RacerState;
        if (racerState == null) {
            Debug.LogError("Racer state not found.");
            return;
        }
        
        if (racerState.CurrCheckpoint.nextCheckpoint == this) racerState.ReachCheckpoint(this);
    }

    /// <summary>
    /// Provides a rotation pointing from the checkpoint forward direction towards the forward direction of the race
    /// </summary>
    /// <returns>Quaternion rotation to the track forward direction</returns>
    public Quaternion GetTrackForwardDirection()
    {
        return transform.rotation * Quaternion.Euler(Vector3.down * 90);
    }
}