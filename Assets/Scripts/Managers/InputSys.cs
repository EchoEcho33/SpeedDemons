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

    [SerializeField]
    public InputActionReference ability;
    [SerializeField]
    public InputActionReference useItem;

    [Header("Keyboard Controls")]
    [SerializeField]
    public InputActionReference accelerate_kbd;

    [SerializeField]
    public InputActionReference brake_kbd;

    [SerializeField]
    public InputActionReference turnRight_kbd;

    [SerializeField]
    public InputActionReference turnLeft_kbd;

    [SerializeField]
    public InputActionReference ability_kbd;

    [SerializeField]
    public InputActionReference useItem_kbd;
    
    [Header("Player 2 Keyboard Controls")]
    [SerializeField]
    public InputActionReference p2_accelerate_kbd;

    [SerializeField]
    public InputActionReference p2_brake_kbd;

    [SerializeField]
    public InputActionReference p2_turnRight_kbd;

    [SerializeField]
    public InputActionReference p2_turnLeft_kbd;

    [SerializeField]
    public InputActionReference p2_ability_kbd;

    [SerializeField]
    public InputActionReference p2_useItem_kbd;
    
    [SerializeField]
    public InputActionReference p2_empty_kbd;
}