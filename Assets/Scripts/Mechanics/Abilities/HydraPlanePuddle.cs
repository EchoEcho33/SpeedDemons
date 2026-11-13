using Unity.VisualScripting;
using UnityEngine;

public class HydraPlanePuddle : MonoBehaviour
{
    [SerializeField]
    private float duration;
    [SerializeField]
    private float spinOutDuration;
    [SerializeField]
    private float puddleWidth;
    [SerializeField]
    private float puddleHeight;

    private void Awake()
    {
        this.gameObject.transform.localScale = new Vector3(puddleHeight, 0, puddleWidth); // Scales Object to Declared Width/Height
    }
    // Update is called once per frame
    void Update()
    {
        // Destroys Puddle After Duration
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    // Trigger for Racer Collision to Cause Spin Out
    private void OnTriggerEnter(Collider other)
    {
        Transform parentTransform = other.gameObject.transform.root;
        // Exit if the collision was not triggered by a Racer
        if (!parentTransform.CompareTag("Racer")) { return; }

        // Attempt to get Drive Component & Call SpinOut
        if (parentTransform.TryGetComponent<Drive>(out var drive))
        {
           drive.Racer.RacerState.ApplyDebuff(RacerState.Debuff.SPINOUT);
        }

        // Delete After Collision
        Destroy(this.gameObject);
    }
}