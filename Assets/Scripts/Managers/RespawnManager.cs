using System;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public GameObject deathFloor;

    public void Respawn(Character player)
    {
        Debug.Log("Respawning player");
        TrackCheckpoint currentCheckpoint = player.racerState.currCheckpoint;
        Vector3 spawnPosition = currentCheckpoint.transform.position + Vector3.up * 5;
        //player.transform.position = spawnPosition;
    }
}
