using UnityEngine;
using UnityEngine.InputSystem;

public class InputSys : MonoBehaviour
{
    [Header("Controller Controls")]
    [SerializeField]
    public InputActionReference accelerate;

    [SerializeField]
    public InputActionReference brake;

    [SerializeField]
    public InputActionReference turn;

    [Header("Keyboard Controls")]
    [SerializeField]
    public InputActionReference accelerate_kbd;

    [SerializeField]
    public InputActionReference brake_kbd;

    [SerializeField]
    public InputActionReference turnRight_kbd;

    [SerializeField]
    public InputActionReference turnLeft_kbd;
}