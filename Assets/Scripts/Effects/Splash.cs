using System.Collections;
using UnityEngine;

public class Splash : MonoBehaviour
{
    [SerializeField]
    private GameObject _splashEffect;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(PlaySplash(Instantiate(_splashEffect, other.gameObject.transform.position, Quaternion.identity)));
        }
    }

    private IEnumerator PlaySplash(GameObject splashObj)
    {
        ParticleSystem[] splashes = splashObj.GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem splash in splashes)
        {
            splash.Play();
        }
        yield return new WaitForSeconds(2.0f);

        Destroy(splashObj);
    }
}
