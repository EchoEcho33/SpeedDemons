using UnityEngine;

public class TrolleyScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("AI"))
        {
            // Component should be safe considering nothing else uses this tag -Gianluca
            Drive collisionRacer = other.gameObject.GetComponent<Drive>();
            
            if (!collisionRacer.Racer.SpinOut)
            {
                // play animation
                if (collisionRacer.gameObject.transform.Find("Model").TryGetComponent<Animator>(out var animator))
                {
                    animator.SetBool("SpinOut", true);
                }
                collisionRacer.Racer.SpinOut = true;
                collisionRacer.Racer.SpinOutDuration = 3.0f;
            }
            
        }
    }
}
