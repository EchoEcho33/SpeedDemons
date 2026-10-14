using UnityEngine;

public class DeathFloor : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        Drive drive = other.gameObject.GetComponent<Drive>();
        if (drive == null) return;
        
        RacerController racer = drive.Racer;
        if (racer == null) return;
        
        RacerState racerState = racer.RacerState;
        if (racerState == null) return;
            
        racerState.Respawn();
    }
}
