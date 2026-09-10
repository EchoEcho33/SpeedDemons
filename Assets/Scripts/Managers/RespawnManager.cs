using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    private readonly int _spawnVerticalDisplacement = 2;
    
    public void RespawnCharacter(Character racer)
    {
        Debug.Log("Respawning racer");
        TrackCheckpoint currentCheckpoint = racer.racerState.currCheckpoint;
        Vector3 spawnPosition = currentCheckpoint.transform.position + Vector3.up * _spawnVerticalDisplacement;
        GameObject racerAndCar = racer.GetRacerAndCar();
        Rigidbody racerAndCarRigidbody = racerAndCar.GetComponent<Rigidbody>();
        racerAndCarRigidbody.linearVelocity = Vector3.zero;
        racerAndCarRigidbody.angularVelocity = Vector3.zero;
        racerAndCar.transform.position = spawnPosition;
        racerAndCar.transform.rotation = currentCheckpoint.GetTrackForwardDirection();
    }
}
