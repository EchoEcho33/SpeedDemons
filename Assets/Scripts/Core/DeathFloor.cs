using UnityEngine;

public class DeathFloor : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        Character racer = other.gameObject.GetComponentInParent<Drive>().character;
        if (racer != null)
        {
            Debug.Log("Respawning racer");
            racer.Respawn();
        }
    }
}
