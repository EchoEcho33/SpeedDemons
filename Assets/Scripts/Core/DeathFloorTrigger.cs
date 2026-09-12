using UnityEngine;

public class DeathFloorTrigger : MonoBehaviour
{
    private RespawnManager respawnManager;

    public void Start()
    {
        respawnManager = GameManager.Instance.respawnManager;
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entering trigger with object of type: " + other.GetType());
        
        Drive drive = other.gameObject.GetComponentInParent<Drive>();
        if (drive == null) return;
        
        RacerController racer = drive.Racer;
        if (racer != null)
        {
            Debug.Log("Respawning " + other.gameObject.name);
            respawnManager.RespawnRacer(racer);
        }
    }
}
