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
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("AI"))
        {
            Drive collisionRacer = other.gameObject.GetComponent<Drive>();
            
            if (!collisionRacer.Racer.SpinOut)
            {
                collisionRacer.Racer.RacerState.ApplyDebuff(RacerState.Debuff.SPEEDREDUCTION, 1000000f, 0.75f);
            }
            
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("AI"))
        {
            Drive collisionRacer = other.gameObject.GetComponent<Drive>();
            
            if (!collisionRacer.Racer.SpinOut)
            {
                collisionRacer.Racer.RacerState.ApplyBuff(RacerState.Buff.SPEEDBOOST, 1000000f, 1.00001f);
            }
            
        }
    }

}
