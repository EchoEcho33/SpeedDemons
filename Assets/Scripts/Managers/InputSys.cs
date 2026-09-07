using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSys : MonoBehaviour
{
    [SerializeField]
    public InputActionReference accelerate;

    [SerializeField]
    public InputActionReference brake;

    [SerializeField]
    public InputActionReference turn;
}