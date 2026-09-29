using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Snowball : Item
{

    private float duration = 10.0f;
    private float maxsize = 10;
    private float growthscale = 1 / 40000f;
    private float startsize = 0.2f;
    private float basefollowr = 5;

    private CinemachineOrbitalFollow follow;
    private SphereCollider sphereCollider;
    private GameObject sphereObject;
    private Vector3 _sphereObjectScale;

    private Coroutine _activeTimer;


    private void Start()
    {
        follow = GameManager.Instance.cinemachineCameraPrefab.GetComponent<CinemachineOrbitalFollow>();
        if (follow == null)
            Debug.LogError("No Cinemachine Orbital Follow found in GameManager.");
    }

    public override void Use(RacerState racer)
    {
        //if (_snowballActive) return;

        if (follow != null) basefollowr = follow.Radius;

        // Enable snowball
        sphereCollider = racer.RacerController.GetCharacterAndKart().GetComponentInChildren<SphereCollider>(true);
        sphereObject = sphereCollider.gameObject;
        
        //apply starting size
        sphereObject.transform.localScale = new Vector3(startsize, startsize, startsize);

        sphereObject.SetActive(true);
    }


    void Update()
    {
        if (sphereObject.activeSelf)
        {
            //sphere growth based on speed
            sphereObject.transform.localScale += new Vector3(growthscale, growthscale, growthscale);
            
            //idea: insert rotate ball or increasing mass

            if (follow != null) follow.Radius = basefollowr + sphereObject.transform.localScale.x;
            
            //if too big, stop as well
            if (sphereObject.transform.localScale.x == maxsize)
            {
                StopCoroutine(_activeTimer);
                sphereObject.SetActive(false);
                if (follow != null) follow.Radius = basefollowr;
            }
        }

    }

    private IEnumerator BecomeSnowball()
    {
        yield return new WaitForSeconds(duration);
        sphereObject.SetActive(false);
    }

    //event for hitting other drivers
    private void OnTriggerEnter(Collider other)
    {
        Drive drive = other.gameObject.GetComponentInParent<Drive>();
        if (drive == null) return;

        RacerController racer = drive.Racer;
        if (racer == null)
        {
            Debug.LogError("Racer not found.");
            return;
        }

        //placeholder for spinout
        racer.SpinOut = true;
        racer.SpinOutDuration = 3.0f;
    }
}