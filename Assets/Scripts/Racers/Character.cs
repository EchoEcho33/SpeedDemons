using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private float _maxSpeed = 25.0f;

    [SerializeField]
    private float _maxAcceleration = 10.0f;

    [SerializeField]
    private float _brakeStrength = 10.0f;

    [SerializeField]
    private float _drag = 4.0f;

    [SerializeField, Range(0, 75)]
    private int _turnRadius;

    [SerializeField]
    private float _traction;

    private Drive drive;
    
    public RacerState racerState {private set; get;}

    public void Start()
    {
        drive = gameObject.GetOrAddComponent<Drive>();
        drive.Init(_maxSpeed, _maxAcceleration, _brakeStrength, _drag, _turnRadius, _traction);
        
    }

    public void AssignRacerState(RacerState newRacerState)
    {
        racerState = newRacerState;
    }
}