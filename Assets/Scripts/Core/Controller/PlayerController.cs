using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;
using FMODUnity;

public class PlayerController : RacerController
{
    public Camera MainCamera { get;  set; }
    
    public PlayerInput input;

    public RaceUI playerUI; 
    
    private InputAction accelerate;

    private InputAction brake;

    private InputAction turn; 

    private InputAction ability;

    private InputAction useItem;

    private InputAction trick;
    
    public override void InitializeDrive()
    {
        Drive = Kart.kartObject.AddComponent<Drive>();
        Drive.AssignController(this);
        Drive.Initialize(Character, Kart);

        // manages inputs for now
        input = Kart.kartObject.GetComponent<PlayerInput>();
        
        Debug.Log(input.currentControlScheme);
        
        // refactor all to use input.actions
        accelerate = input.actions.FindAction("Accelerate");
        Debug.Log("Accelerate is equal to" + accelerate);
        brake = input.actions.FindAction("Brake");
        turn = input.actions.FindAction("Turn");
        
        ability = input.actions.FindAction("Ability");

        useItem = input.actions.FindAction("UseItem");

        trick = input.actions.FindAction("Trick");
    }
    
    private void FixedUpdate()
    {
        
        var old_transform_position = transform.position;
        // Spin Out Handler
        if (base.SpinOut)
        {
            // Main Spin Out Code
            Drive.SpinOut();
            base.SpinOutDuration -= Time.fixedDeltaTime;

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
            immobileDuration -= Time.fixedDeltaTime;
            
            if (base.immobileDuration <= 0)
            {
                base.preventMovement = false;
            }

            return; // prevent any movement in update
        }

        // Check if the kart's grounded.
        Drive.CheckGrounded();
        // Normal Rotation.
        Drive.setNormalRotation();

        // Brake and Accelerate
        if (accelerate.IsPressed() || brake.IsPressed())
        {
            ControllerMove();
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
            if (accelerate.IsPressed() && brake.IsPressed())
            {
                Drive.Turn(Steering, 1.5f);
            }
            else
            {
                Drive.Turn(Steering); 
            }
        } 

        // Trick
        if (Drive.hasTricked && Drive.isGrounded)
        {
            Drive.applyTrickMult(0.5f, 1.2f);
        }

        if (trick.IsPressed())
        {
            if (turn.IsPressed()) {
                int dir = turn.ReadValue<Vector2>().x > 0 ? 1 : -1;
                Drive.Trick(dir);
            }
            else
            {
            Drive.Trick();
            }
        }

        // Ability
        if (ability.IsPressed())
        {
            // The Ability Trigger is Not Fully Set Up, So this goes to Comment Jail for now
            // Character.TriggerAbility();
        }

        if (useItem.IsPressed())
        {
            RacerState.UseItem();
        }
        
        float up_dot = Vector3.Dot(transform.up, new Vector3(0,1,0));
        if (up_dot <= 1.0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.forward,new Vector3(0,1,0)), Time.fixedDeltaTime * 5);
        }
        
        transform.position = new Vector3(transform.position.x, old_transform_position.y - (9.81f * Time.fixedDeltaTime), transform.position.z);
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

    
}