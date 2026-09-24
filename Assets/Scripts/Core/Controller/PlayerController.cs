using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : RacerController
{
    public Camera MainCamera { get; private set; }
    public CinemachineCamera CinemachineCamera { get; private set; }
    
    private InputAction accelerate;

    private InputAction brake;

    private InputAction turn; 

    private InputAction accelerate_kbd;
    
    private InputAction brake_kbd;
    
    private InputAction turnRight_kbd;
    
    private InputAction turnLeft_kbd;

    private InputAction ability;

    private InputAction ability_kbd;
        
    public override void AssignCharacterAndKart(Character newCharacter, Kart newKart)
    {
        base.AssignCharacterAndKart(newCharacter, newKart);
        InitializeCamera();
    }

    protected override void InitializeDrive()
    {
        Drive = Kart.kartObject.AddComponent<Drive>();
        Drive.AssignController(this);
        Drive.Initialize(Character, Kart);
        
        accelerate = GameManager.Instance.inputManager.accelerate.action;
        brake = GameManager.Instance.inputManager.brake.action;
        turn = GameManager.Instance.inputManager.turn.action;

        accelerate_kbd = GameManager.Instance.inputManager.accelerate_kbd.action;
        brake_kbd = GameManager.Instance.inputManager.brake_kbd.action;
        turnRight_kbd = GameManager.Instance.inputManager.turnRight_kbd.action;
        turnLeft_kbd = GameManager.Instance.inputManager.turnLeft_kbd.action;
        
        ability = GameManager.Instance.inputManager.ability.action;
        ability_kbd = GameManager.Instance.inputManager.ability_kbd.action;
    }

    private void InitializeCamera()
    {
        GameObject cameraObject = Instantiate(GameManager.Instance.cameraPrefab);
        MainCamera = cameraObject.GetComponent<Camera>();
        
        GameObject cinemachineCameraObject = Instantiate(GameManager.Instance.cinemachineCameraPrefab);
        CinemachineCamera = cinemachineCameraObject.GetComponent<CinemachineCamera>();
        CinemachineCamera.Follow = Kart.kartObject.transform;
    }

    private void Update()
    {
        // Spin Out Handler
        if (base.SpinOut)
        {
            // Main Spin Out Code
            Drive.SpinOut();
            base.SpinOutDuration -= Time.deltaTime;

            // Ends Spin Out Animation
            if (SpinOutDuration <= 1 && Drive.gameObject.transform.Find("Model").TryGetComponent<Animator>(out var animator))
            {
                animator.SetBool("SpinOut", false);
            }

            // Ends Spin Out
            if (SpinOutDuration <= 0) { base.SpinOut = false; }

            return; // Exit Update Code During Spin Out
        }

        // Brake and Accelerate
        if (accelerate.IsPressed() || brake.IsPressed())
        {
            ControllerMove();
        } 
        else if (accelerate_kbd.IsPressed() || brake_kbd.IsPressed())
        {
            KeyboardMove();
        } 
        else if (Drive.GetCurrentSpeed() > 0)
        {
            Drive.Decelerate(1.0f, Kart._drag);
        } 
        else if (Drive.GetCurrentSpeed() < 0)
        {
            Drive.Accelerate(0.5f);
        }

        // Steering
        if (turn.IsPressed())
        {
            Steering = turn.ReadValue<Vector2>().x;
            Drive.Turn(Steering);
        } 
        else if (turnRight_kbd.IsPressed())
        {
            Steering = 1.0f;
            Drive.Turn(Steering);
        } 
        else if (turnLeft_kbd.IsPressed())
        {
            Steering = -1.0f;
            Drive.Turn(Steering);
        }

        // Ability
        if (ability.IsPressed() || ability_kbd.IsPressed())
        {
            // The Ability Trigger is Not Fully Set Up, So this goes to Comment Jail for now
            // Character.TriggerAbility();
        }
    }

    private void ControllerMove()
    {
        if (accelerate.IsPressed())
        {
            Throttle = accelerate.GetControlMagnitude();
            Drive.Accelerate(Throttle);
        }
        else if (brake.IsPressed())
        {
            Brake = brake.GetControlMagnitude();
            if (Drive.GetCurrentSpeed() <= 0)
            {
                Drive.Accelerate(-brake.GetControlMagnitude() / 2);
            }
            else
            {
                Drive.Decelerate(Brake, Kart._brakeStrength);
            }
        }
    }

    private void KeyboardMove()
    {
        if (accelerate_kbd.IsPressed())
        {
            Throttle = 1.0f;
            Drive.Accelerate(Throttle);
        } 
        else if (brake_kbd.IsPressed())
        {
            Brake = 1.0f;
            if (Drive.GetCurrentSpeed() <= 0)
            {
                Drive.Accelerate(-0.5f);
            } 
            else
            {
                Drive.Decelerate(Brake, Kart._brakeStrength);
            }
        }
    }
}