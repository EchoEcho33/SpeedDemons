using System.Collections;
using UnityEngine;

public class SpinOut : MonoBehaviour
{

    private float delaySecs = 1.5f;
    private bool checkable = true;

    private void OnCollisionEnter(Collision other)
    {
        if (checkable && (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("AI")))
        {
            StartCoroutine(CollisionCheckDelay());

            // Component should be safe considering nothing else uses this tag -Gianluca
            Drive collisionRacer = other.gameObject.GetComponent<Drive>();
            
            if (!collisionRacer.Racer.SpinOut)
            {
                collisionRacer.Racer.RacerState.ApplyDebuff(RacerState.Debuff.SPINOUT);
            }
            
        }
    }

    private IEnumerator CollisionCheckDelay()
    {
        checkable = false;
        yield return new WaitForSeconds(delaySecs);
        checkable = true;
    }

}
