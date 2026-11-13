using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

[RequireComponent(typeof(SphereCollider))]
public class OutOfBoundsSlowdown : MonoBehaviour
{
    private SphereCollider sphereCollider;
    private bool inCollider;

    private void OnDrawGizmos()
    {
        if (sphereCollider == null)
            sphereCollider = gameObject.GetComponent<SphereCollider>();

        Gizmos.color = Color.azure;
        Gizmos.DrawWireSphere(gameObject.transform.position, sphereCollider.radius);
    }

    public void OnTriggerEnter(Collider other)
    {
        Drive collisionRacer = other.gameObject.GetComponent<Drive>();

        if (collisionRacer != null)
        {
            if (!collisionRacer.Racer.SpinOut)
            {
                collisionRacer.Racer.RacerState.GrassSlowdown(0.75f);
            }
            
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Drive collisionRacer = other.gameObject.GetComponent<Drive>();

        if (collisionRacer != null)
        {
            if (!collisionRacer.Racer.SpinOut)
            {
                collisionRacer.Racer.RacerState.RemoveSpeedDebuff();
            }
            
        }
    }

}
