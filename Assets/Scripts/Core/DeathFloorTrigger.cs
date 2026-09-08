using UnityEngine;

public class DeathFloorTrigger : MonoBehaviour
{
    public RespawnManager respawnManager;

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entering trigger with object of type: " + other.GetType());
        RigidBody player = other.attachedRigidBody;
        if (player != null)
        {

            respawnManager.Respawn(player);
        }
    }
}
