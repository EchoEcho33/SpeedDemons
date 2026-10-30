using UnityEngine;

public class SpinOutTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Drive kartDrive = other.GetComponent<Drive>();

        print(other.gameObject.name);
        if (kartDrive != null)
        {
            Debug.Log("It hit!");
            Debug.Log(kartDrive.gameObject.tag);

            kartDrive.SpinOut();
        }
    }
}
