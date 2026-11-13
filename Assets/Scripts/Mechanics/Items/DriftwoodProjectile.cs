using System;
using UnityEngine;
using Unity.Collections;

public class DriftwoodProjectile : MonoBehaviour
{

    [Header("Driftwood Projectile Attributes")]
    
    
    [ReadOnly] private float speed = 50;
    [ReadOnly] private float distance = 150f;
    [ReadOnly] private float heightAboveTrack = 1.5f;
    [ReadOnly] private Vector3 forwardDirection = new Vector3(0, 0, 0);
    RacerState userRacerState;
    private float distanceTraveled = 0;
    private bool isReturning = false;

    void Awake()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + heightAboveTrack, transform.position.z);
    }
    void Update()
    {
        if (isReturning)
        {
            returnMotion();
        } else
        {
            forwardMotion();
        }
        adjustHeight();
    }

    // Trigger for Racer Collision to Cause Spin Out
    private void OnTriggerEnter(Collider other)
    {

        // Parent Transform of Collided Object
        Transform parentTransform = other.gameObject.transform.root;

        // Exit if the collision was not triggered by a Racer
        if (!parentTransform.CompareTag("Player")) { 
            isReturning = true;
            return; 
        }

        // Exit if Collision was with the player who threw the object
        if (parentTransform.gameObject.name == userRacerState.RacerController.GetCharacterAndKart().name) { return; }

        // Attempt to get Drive Component & Call SpinOut
        if (parentTransform.TryGetComponent<Drive>(out var drive))
        {
            RacerState targetRacerState = drive.Racer.RacerState;
            if (targetRacerState != null)
            {
                targetRacerState.ApplyDebuff(RacerState.Debuff.SPINOUT);
            }
        }
    }

    /** 
    * Handles the Forward Motion of the projectile
    */
    private void forwardMotion()
    {
        gameObject.transform.position += forwardDirection * speed * Time.deltaTime;
        distanceTraveled += speed * Time.deltaTime;
        if (distanceTraveled >= distance)
        {
            isReturning = true;
        }
    }

    /** 
    * Handles the Return Motion of the projectile
    */
    private void returnMotion() {

        // Calculate Player Position and Direction to Travel for Return
        Vector3 playerPosition = userRacerState.RacerController.GetCharacterAndKart().transform.position;
        Vector3 returnDir = Vector3.Normalize(playerPosition - transform.position);

        // Move towards player
        gameObject.transform.position += returnDir * speed * Time.deltaTime;

        // Destroy Projectile Once it returns to player
        if (Math.Pow(Math.Pow(playerPosition.x - transform.position.x, 2) + Math.Pow(playerPosition.z - transform.position.z, 2), .5) <= 1)
        {
            Destroy(this.gameObject);
        }
    }

    /** 
    * Forces Projectile to Remain Above the Track
    */
    private void adjustHeight()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, heightAboveTrack, 1 << LayerMask.NameToLayer("GroundLayer")))
        {
            transform.position = new Vector3 (transform.position.x, hit.point.y + heightAboveTrack, transform.position.z);
        }
    }

    /* ==== SETTERS ==== */
    
    public void SetForwardDirection(Vector3 dir)
    {
        forwardDirection = dir;
    }

    public void SetSpeed(float spd)
    {
        speed = spd;
    }

    public void SetDistance(float dist)
    {
        distance = dist;
    }

    public void SetRacer (RacerState racer)
    {
        userRacerState = racer;
    }

}
