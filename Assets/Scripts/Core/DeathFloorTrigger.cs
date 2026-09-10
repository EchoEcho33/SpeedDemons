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
        
        // TODO (Taylor) - may not work when we get AI implemented, as the Drive script is for player control
        //      suggestion - create a map of Racers to RacerState objects in GameManager & register on start?
        Character racer = other.gameObject.GetComponentInParent<Drive>().getCharacter();
        if (racer != null)
        {
            Debug.Log("Respawning " + other.gameObject.name);
            respawnManager.RespawnCharacter(racer);
        }
    }
}
