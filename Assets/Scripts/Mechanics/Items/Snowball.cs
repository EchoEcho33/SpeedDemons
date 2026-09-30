using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Snowball : Item
{
    [SerializeField]
    private float maxsize = 3;
    [SerializeField]
    private float growthscale = 0.2f;
    [SerializeField]
    private float startsize = 2f;
    private float basefollowr = 5;

    private RacerState racer;

    private CinemachineOrbitalFollow follow;
    private SphereCollider sphereCollider;
    private GameObject sphereObject;


    private void Start()
    {
        follow = GameManager.Instance.cinemachineCameraPrefab.GetComponent<CinemachineOrbitalFollow>();
        if (follow == null)
            Debug.LogError("No Cinemachine Orbital Follow found in GameManager.");
    }

    public override void Use(RacerState racer)
    {
        if (follow != null) basefollowr = follow.Radius;

        this.racer = racer;

        // Enable snowball
        sphereCollider = racer.RacerController.GetCharacterAndKart().GetComponentInChildren<SphereCollider>(true);
        sphereObject = sphereCollider.gameObject;

        sphereObject.SetActive(true);
    }


    void Update()
    {
        if (sphereObject.activeSelf)
        {
            // Grow sphere based on growth scale
            sphereObject.transform.localScale += new Vector3(growthscale, growthscale, growthscale) * Time.deltaTime;
            sphereObject.transform.rotation *= Quaternion.AngleAxis(-10 * racer.RacerController.Drive.GetCurrentSpeed() * Time.deltaTime, 
                                            Quaternion.LookRotation(racer.RacerController.gameObject.transform.forward, 
                                            racer.RacerController.gameObject.transform.up) * Vector3.right);

            if (follow != null) follow.Radius = basefollowr + sphereObject.transform.localScale.x;
            
            // When the sphere reaches it's maximum size threshold, player returns to normal
            if (sphereObject.transform.localScale.x >= maxsize)
            {
                sphereObject.SetActive(false);
                if (follow != null) follow.Radius = basefollowr;
                sphereObject.transform.localScale = new Vector3(startsize, startsize, startsize);
            }
        }

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