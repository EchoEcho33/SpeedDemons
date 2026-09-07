using UnityEngine;

[ExecuteInEditMode]
public class StartingGridSpot : MonoBehaviour
{
    public Vector3 visualBox = new(4f, 1.5f, 2f);
    
#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        if (Application.isPlaying) return;

        Vector3 boxCenter = GetSpawnPoint();
        
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.DrawCube(boxCenter,visualBox);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, visualBox);
    }
#endif

    public Vector3 GetSpawnPoint()
    {
        Vector3 boxCenter = new(transform.position.x, transform.position.y + visualBox.y / 2, transform.position.z);
        return boxCenter;
    }
}
