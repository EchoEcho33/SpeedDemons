using System.Collections;
using UnityEngine;

public class TestCarHonk : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(waitForHonk());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator waitForHonk()
    {
        yield return new WaitForSeconds(10);
        AudioManager.instance.PlayOneShot(AudioEvents.instance.seanHonk, transform.position);
        Debug.Log("honked");
        StartCoroutine(waitForHonk());
    }
}
