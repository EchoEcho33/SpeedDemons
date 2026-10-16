using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;
using FMODUnity;

public class PlayerController : RacerController
{
    public Camera MainCamera { get; private set; }
    public CinemachineCamera CinemachineCamera;
    
    private InputAction accelerate;

    private InputAction brake;

    private InputAction turn; 

    private InputAction accelerate_kbd;
    
    private InputAction brake_kbd;
    
    private InputAction turnRight_kbd;
    
    private InputAction turnLeft_kbd;

    private InputAction ability;

    private InputAction ability_kbd;

    private InputAction useItem;
    private InputAction useItem_kbd;
    
    public int playerNum = 0;

    

    public override void InitializeDrive()
    {
        Drive = Kart.kartObject.AddComponent<Drive>();
        Drive.AssignController(this);
        Drive.Initialize(Character, Kart);
        
        // INSANELY HARD CODED.
        // TODO - Gianluca: Refactor using input manager to avoid having to do this for each additional player
        // There is a built in split screen thing that can work, but our current UI system would have to change
        
        if (playerNum == 0)
        {
            accelerate = GameManager.Instance.inputManager.accelerate.action;
            brake = GameManager.Instance.inputManager.brake.action;
            turn = GameManager.Instance.inputManager.turn.action;

            accelerate_kbd = GameManager.Instance.inputManager.accelerate_kbd.action;
            brake_kbd = GameManager.Instance.inputManager.brake_kbd.action;
            turnRight_kbd = GameManager.Instance.inputManager.turnRight_kbd.action;
            turnLeft_kbd = GameManager.Instance.inputManager.turnLeft_kbd.action;
        
            ability = GameManager.Instance.inputManager.ability.action;
            ability_kbd = GameManager.Instance.inputManager.ability_kbd.action;

            useItem = GameManager.Instance.inputManager.useItem.action;
            useItem_kbd = GameManager.Instance.inputManager.useItem_kbd.action;
        }
        else
        {
            accelerate_kbd = GameManager.Instance.inputManager.p2_accelerate_kbd.action;
            brake_kbd = GameManager.Instance.inputManager.p2_brake_kbd.action;
            turnRight_kbd = GameManager.Instance.inputManager.p2_turnRight_kbd.action;
            turnLeft_kbd = GameManager.Instance.inputManager.p2_turnLeft_kbd.action;
            
            ability_kbd = GameManager.Instance.inputManager.ability_kbd.action;

            
            useItem_kbd = GameManager.Instance.inputManager.p2_useItem_kbd.action;
            
            // undef values. will get second controller later
            accelerate = GameManager.Instance.inputManager.p2_empty_kbd;
            brake = GameManager.Instance.inputManager.brake.action;
            turn = GameManager.Instance.inputManager.turn.action;
            ability = GameManager.Instance.inputManager.ability.action;
            useItem = GameManager.Instance.inputManager.useItem.action;
        }
        
    }

    public void InitializeSingleCamera()
    {
            GameObject cameraObject = Instantiate(GameManager.Instance.cameraPrefab);
            MainCamera = cameraObject.GetComponent<Camera>();
    
            GameObject cinemachineCameraObject = Instantiate(GameManager.Instance.cinemachineCameraPrefab);
            CinemachineCamera = cinemachineCameraObject.GetComponent<CinemachineCamera>();
            CinemachineCamera.Follow = Kart.kartObject.transform;

            // audio stuff
            MainCamera.GetComponent<StudioListener>().SetAttenuationObject(MainCamera.gameObject);
    }

    public void InitializeMultiCamera(int playerIndex)
    {
        // this is hardcoded and bad. currently two player
        if (playerIndex == 0)
        {
            GameObject cameraObject = Instantiate(GameManager.Instance.cameraPrefab);
            MainCamera = cameraObject.GetComponent<Camera>();
            CinemachineBrain brain = MainCamera.GetComponent<CinemachineBrain>();
            brain.ChannelMask = OutputChannels.Channel01;
            MainCamera.rect = new Rect(0f, 0.5f, 1f, 0.5f);

    
            GameObject cinemachineCameraObject = Instantiate(GameManager.Instance.cinemachineCameraPrefab);
            CinemachineCamera = cinemachineCameraObject.GetComponent<CinemachineCamera>();
            CinemachineCamera.OutputChannel = OutputChannels.Channel01;
            CinemachineCamera.Follow = Kart.kartObject.transform;

            // audio stuff
            MainCamera.GetComponent<StudioListener>().SetAttenuationObject(MainCamera.gameObject);
        }
        else
        {
            GameObject cameraObject2 = Instantiate(GameManager.Instance.cameraPrefab);
            MainCamera = cameraObject2.GetComponent<Camera>();
            CinemachineBrain brain = MainCamera.GetComponent<CinemachineBrain>();
            brain.ChannelMask = OutputChannels.Channel02;
            MainCamera.rect = new Rect(0f, 0f, 1f, 0.5f);
            
            GameObject cinemachineCameraObject = Instantiate(GameManager.Instance.cinemachineCameraPrefab);
            CinemachineCamera = cinemachineCameraObject.GetComponent<CinemachineCamera>();
            CinemachineCamera.OutputChannel = OutputChannels.Channel02;
            CinemachineCamera.Follow = Kart.kartObject.transform;
        }
    }
    private void FixedUpdate()
    {
        var old_transform_position = transform.position;
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
        
        // Race Countdown Handler
        // see PassInputs in AIController for equivalent
        if (base.preventMovement)
        {
            Drive.PreventMovement();
            immobileDuration -= Time.deltaTime;
            
            if (base.immobileDuration <= 0)
            {
                base.preventMovement = false;
            }

            return; // prevent any movement in update
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

        if (useItem.IsPressed() || useItem_kbd.IsPressed())
        {
            RacerState.UseItem();
        }
        
        float up_dot = Vector3.Dot(transform.up, new Vector3(0,1,0));
        if (up_dot <= 1.0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.forward,new Vector3(0,1,0)), Time.deltaTime * 5);
        }
        
        transform.position = new Vector3(transform.position.x, old_transform_position.y - (9.81f * Time.deltaTime), transform.position.z);
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