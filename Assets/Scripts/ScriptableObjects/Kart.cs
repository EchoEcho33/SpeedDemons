using UnityEngine;

[CreateAssetMenu(fileName = "Kart", menuName = "Scriptable Objects/Kart")]
public class Kart : ScriptableObject
{
    [SerializeField]
    public float _maxSpeed = 25.0f;

    [SerializeField]
    public float _maxAcceleration = 10.0f;

    [SerializeField]
    public float _brakeStrength = 10.0f;

    [SerializeField]
    public float _drag = 4.0f;

    [SerializeField, Range(0, 75)]
    public int _turnRadius;

    [SerializeField]
    public float _traction;
}
