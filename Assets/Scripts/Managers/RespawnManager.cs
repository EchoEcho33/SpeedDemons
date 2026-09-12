using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    private readonly int _spawnVerticalDisplacement = 2;

    public void Start()
    {
        // Check for death floor asset
        GameObject deathFloor =  GameObject.Find("DeathFloor");
        if (deathFloor == null)
        {
            throw new System.Exception("RespawnManager cannot find death floor - make sure one is placed below the track and spans the entire area");
        }
    }
    
    /// <summary>
    /// Respawn the racer at the most recent checkpoint reached
    /// </summary>
    /// <param name="racer">The racer to respawn. This could be a player or AI racer</param>
    public void RespawnCharacter(Character racer)
    {
        Debug.Log("Respawning racer");
        TrackCheckpoint currentCheckpoint = racer.racerState.currCheckpoint;
        GameObject racerAndCar = racer.GetRacerAndCar();
        Rigidbody racerAndCarRigidbody = racerAndCar.GetComponent<Rigidbody>();
        racerAndCarRigidbody.linearVelocity = Vector3.zero;
        racerAndCarRigidbody.angularVelocity = Vector3.zero;
        racerAndCar.transform.position = currentCheckpoint.transform.position + Vector3.up * _spawnVerticalDisplacement;
        racerAndCar.transform.rotation = currentCheckpoint.GetTrackForwardDirection();
    }
}
