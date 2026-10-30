using UnityEngine;

public class TrolleyScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("AI"))
        {
            // Component should be safe considering nothing else uses this tag -Gianluca
            RacerController collisionRacer = other.gameObject.GetComponent<Drive>().Racer;
            if (!collisionRacer.SpinOut)
            {
                collisionRacer.SpinOut = true;
                collisionRacer.SpinOutDuration = 3.0f;    
            }
            
        }
    }
}
