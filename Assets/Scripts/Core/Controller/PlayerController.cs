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
        
        accelerate = GameManager.Instance.input.accelerate.action;
        brake = GameManager.Instance.input.brake.action;
        turn = GameManager.Instance.input.turn.action;

        accelerate_kbd = GameManager.Instance.input.accelerate_kbd.action;
        brake_kbd = GameManager.Instance.input.brake_kbd.action;
        turnRight_kbd = GameManager.Instance.input.turnRight_kbd.action;
        turnLeft_kbd = GameManager.Instance.input.turnLeft_kbd.action;
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
            Drive.Turn(turn.ReadValue<Vector2>().x);
        } 
        else if (turnRight_kbd.IsPressed())
        {
            Drive.Turn(1.0f);
        } 
        else if (turnLeft_kbd.IsPressed())
        {
            Drive.Turn(-1.0f);
        }
    }

    private void ControllerMove()
    {
        if (accelerate_kbd.IsPressed())
        {
            Drive.Accelerate(1.0f);
        } 
        else if (brake_kbd.IsPressed())
        {
            if (Drive.GetCurrentSpeed() <= 0)
            {
                Drive.Accelerate(-0.5f);
            } 
            else
            {
                Drive.Decelerate(1.0f, Kart._brakeStrength);
            }
        }
    }

    private void KeyboardMove()
    {
        if (accelerate_kbd.IsPressed())
        {
            Drive.Accelerate(1.0f);
        } 
        else if (brake_kbd.IsPressed())
        {
            if (Drive.GetCurrentSpeed() <= 0)
            {
                Drive.Accelerate(-0.5f);
            } 
            else
            {
                Drive.Decelerate(1.0f, Kart._brakeStrength);
            }
        }
    }
}