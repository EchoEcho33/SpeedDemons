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
        // Exit if the collision was not triggered by a Racer
        if (!other.gameObject.CompareTag("Racer")) { return; }

        // Attempt to get Drive Component & Call SpinOut
        if (other.gameObject.TryGetComponent<Drive>(out var drive))
        {
            drive.Racer.SpinOutDuration = spinOutDuration;
            drive.Racer.SpinOut = true;
        }

        // Attempts to Trigger the Spin Out Animation On Racer
        if (drive.gameObject.transform.Find("Model").TryGetComponent<Animator>(out var animator))
        {
            animator.SetBool("SpinOut", true);
        }

        // Delete After Collision
        Destroy(this.gameObject);
    }
}