using UnityEngine;

public class DeathFloor : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        // TODO (Taylor) - may not work when we get AI implemented, as the Drive script is for player control
        //      suggestion - create a map of Racers to RacerState objects in GameManager & register on start?
        Character racer = other.gameObject.GetComponentInParent<Drive>().getCharacter();
        if (racer != null)
        {
            racer.Respawn();
        }
    }
}
