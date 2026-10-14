using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KartSuspension : MonoBehaviour
{
    [SerializeField] public List<GameObject> wheels;
    public List<Vector3> wheelLocalPositions;
    public float wheelHeight = .5F;

    void Start()
    {
        for(int i = 0; i < wheels.Count; i++)
        {
            wheelLocalPositions.Add(wheels[i].transform.localPosition);
        } 
        wheelHeight = wheels[0].GetComponent<Renderer>().bounds.size.y + .1f;
        
    }
}

