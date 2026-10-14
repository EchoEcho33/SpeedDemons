using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KartSuspension : MonoBehaviour
{
    [SerializeField] public List<GameObject> wheels;
    public List<Vector3> wheelLocalPositions;
    public bool[] wheelGroundedState;
    public float wheelHeight = .5F;
    public Quaternion rotation;
    public bool lockRotation = false;

    void Awake()
    {
        for(int i = 0; i < wheels.Count; i++)
        {
            wheelLocalPositions.Add(wheels[i].transform.localPosition);
        } 
        wheelHeight = wheels[0].GetComponent<Renderer>().bounds.size.y + .1f;
        wheelGroundedState = new bool[wheels.Count];
        rotation = transform.rotation;
    }
    
    void FixedUpdate()
    {
        CheckGroundedWheels();
        ToggleRotationLock();

        if (lockRotation) { 
            Debug.Log(transform.rotation);
            transform.rotation = rotation;
            return; 
        }

        Physics.Raycast(
            transform.position,
            -transform.up,
            out RaycastHit hit);

        Vector3 targetRotation = Vector3.Slerp(transform.up, hit.normal, Time.deltaTime);

        transform.rotation = Quaternion.FromToRotation(transform.up, targetRotation) * transform.rotation;
    }

    public void CheckGroundedWheels()
    {
        for(int i = 0; i < wheels.Count; i++)
        {
            Vector3 wheelSpringDir = wheels[i].transform.up;
            
            Physics.Raycast(
                transform.TransformPoint(wheelLocalPositions[i]),
                -wheelSpringDir,
                out RaycastHit hit,
                wheelHeight / 2);

            wheelGroundedState[i] = hit.collider != null;
        }
    }

    public void ToggleRotationLock()
    {
        for(int i = 0; i < wheelGroundedState.Length; i++)
        {
            if (wheelGroundedState[i]) {
                rotation = transform.rotation;
                lockRotation = false;
                return;
            }
        }
        lockRotation = true;
    }
}

