using System;
using System.Collections;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using Unity.VisualScripting;

public class Drive : MonoBehaviour
{
    public RacerController Racer { get; private set; }
    
    public Character character { get; private set; }

    private Kart kart;

    public float m_currentSpeed = 0.0f;
    private bool isGrounded = false;
    private bool hasTricked = false;
    private RaycastHit groundHitInfo;

    private float m_speedModifier = 1.0f;
    private Coroutine m_resetTimer;
    // private float m_turnSpeed = 0.0f;
    // private bool m_isDrifting = false;
    private int m_driftAngle = 0;
    public StudioEventEmitter drivingAudio;
    private Rigidbody rigidbody;
    public LayerMask ground;

    public void AssignController(RacerController newRacerController)
    {
        Racer = newRacerController;
    }

    public void Initialize(Character newCharacter, Kart newKart)
    {
        character = newCharacter;
        kart = newKart;
        drivingAudio = AudioManager.instance.createEmitter(AudioEvents.instance.basicCarDrive, this.gameObject);
        drivingAudio.Play();
        rigidbody = gameObject.GetComponent<Rigidbody>();
        ground = LayerMask.GetMask("GroundLayer");
    }

    public void Accelerate(float amplitude)
    {
        if (!isGrounded) return;
        
        float initialFrameSpeed = m_currentSpeed;
        m_currentSpeed += kart._maxAcceleration * amplitude * Time.fixedDeltaTime * m_speedModifier;

        if (m_currentSpeed > kart._maxSpeed * m_speedModifier)
        {
            m_currentSpeed = kart._maxSpeed * m_speedModifier;
            m_currentSpeed -= Time.fixedDeltaTime * m_speedModifier * kart._maxAcceleration;
            if (m_currentSpeed < kart._maxSpeed)
            {
                m_currentSpeed = kart._maxSpeed * m_speedModifier;
            }
        }

        if (m_currentSpeed < -kart._maxSpeed * m_speedModifier / 2)
            m_currentSpeed = -kart._maxSpeed * m_speedModifier / 2;

        //float velocityUpdate = (m_currentSpeed + initialFrameSpeed) / 2 * Time.fixedDeltaTime * m_speedModifier;
        //transform.position += transform.forward * velocityUpdate;
        float speed = (m_currentSpeed + initialFrameSpeed) / 2 * m_speedModifier;
        Vector3 forward = rigidbody.rotation * Vector3.forward;

        Vector3 velocity = speed * forward;
        velocity.y = rigidbody.linearVelocity.y;
        rigidbody.linearVelocity = velocity;

        UpdateSound(1);
    }

    public void Decelerate(float amplitude, float a)
    {
        float initialFrameSpeed = m_currentSpeed;
        m_currentSpeed += -a * amplitude * Time.fixedDeltaTime * m_speedModifier;

        if (m_currentSpeed < 0)
            m_currentSpeed = 0;

        //float velocityUpdate = (m_currentSpeed + initialFrameSpeed) / 2 * Time.fixedDeltaTime * m_speedModifier;
        //transform.position += transform.forward * velocityUpdate;
        float speed = (m_currentSpeed + initialFrameSpeed) / 2 * m_speedModifier;
        Vector3 forward = rigidbody.rotation * Vector3.forward;

        Vector3 velocity = speed * forward;
        velocity.y = rigidbody.linearVelocity.y;
        rigidbody.linearVelocity = velocity;
        UpdateSound(-1);
    }

    public void Turn(float turnDirection, float turnMultiplier = 1.0f)
    {
        if (!isGrounded) return;
        if (m_driftAngle != 0)
        {
            OrientTowardsVelocity();
        }

        if (turnDirection != 0)
        {

            //transform.position += Vector3.forward * kart._turnRadius * turnDirection * turnMultiplier * m_currentSpeed * Time.fixedDeltaTime / 100;

            //rigidbody.linearVelocity = kart._turnRadius * turnDirection * turnMultiplier * rigidbody.linearVelocity * Time.fixedDeltaTime / 100;


            // transform.Rotate(0, kart._turnRadius * turnDirection * turnMultiplier * m_currentSpeed * Time.fixedDeltaTime, 0, Space.Self);
            float turnAmount = kart._turnRadius * turnDirection * turnMultiplier * m_currentSpeed * Time.fixedDeltaTime;
            rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(0, turnAmount, 0));

            Vector3 turnVelocity = Vector3.forward * kart._turnRadius * turnDirection * turnMultiplier * m_currentSpeed / 100f;
            rigidbody.linearVelocity += turnVelocity;
        }

        if (turnMultiplier != 1.0f)
        {
            OrientForDrift(turnDirection);
            Debug.Log("Drifting...");
        }
        else if (m_driftAngle != 0)
        {
            CenterOrientation();
        }
        // x = horizontal (- left + right) y = vertical (- down + up)   
    }

    public void SpinOut()
    {
        if (!isGrounded) return;

        float initialFrameSpeed = m_currentSpeed;
        float slowdownSpeed = 20;

        m_currentSpeed -= slowdownSpeed * Time.fixedDeltaTime;

        if (m_currentSpeed < 0)
            m_currentSpeed = 0;

        //float velocityUpdate = (m_currentSpeed + initialFrameSpeed) / 2 * Time.fixedDeltaTime;
        //transform.position += transform.forward * velocityUpdate;
        float speed = (m_currentSpeed + initialFrameSpeed) / 2;
        Vector3 forward = rigidbody.rotation * Vector3.forward;

        Vector3 velocity = speed * forward;
        velocity.y = rigidbody.linearVelocity.y;
        rigidbody.linearVelocity = velocity;
    }

    // Just left or right for now.
    public void Trick() {
        if (isGrounded) return;
        hasTricked = true;
    }

    public void PreventMovement()
    {
        if (!isGrounded) return;
        m_currentSpeed = 0;
    }

    // TODO: Grounded function is set for now at a set height.
    // Make it customizable for different karts at different heights.
    public void CheckGrounded()
    {
        // Debug.Log("This is being called" + transform.position);

        Vector3 down = Vector3.down;

        Ray groundCheck = new Ray(transform.position + transform.up * 0.8f, down);
        // Debug.DrawRay(transform.position + transform.up * 0.8f, down * 1.6f, Color.red, 0.25f);
        isGrounded = Physics.Raycast(groundCheck, out groundHitInfo, 1.6f, ground);
        // print(groundHitInfo.collider);
        // Add mask for ground layer.
    }

    public void setNormalRotation()
    {
        if (!isGrounded) return;
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, groundHitInfo.normal).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(forward, groundHitInfo.normal);

        rigidbody.MoveRotation(
            Quaternion.Slerp(
                rigidbody.rotation,
                targetRotation,
                Time.fixedDeltaTime * 4f
            )
        );
    }

    public float GetCurrentSpeed()
    {
        return m_currentSpeed;
    }

    private void OrientTowardsVelocity()
    {
        // transform.Rotate(0, -m_driftAngle / 10.0f, 0, Space.Self);
        rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(0, -m_driftAngle / 10.0f, 0));
    }

    private void OrientForDrift(float turnDirection)
    {
        if (Math.Abs(m_driftAngle) < 300)
        {
            if (turnDirection > 0)
            {
                ++m_driftAngle;
            }
            else
            {
                --m_driftAngle;
            }
        }

        // transform.Rotate(0, m_driftAngle / 10.0f, 0, Space.Self);
        rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(0, m_driftAngle / 10.0f, 0));
    }

    private void CenterOrientation()
    {
        if (m_driftAngle > 0)
        {
            --m_driftAngle;
        }
        else
        {
            ++m_driftAngle;
        }

        // transform.Rotate(0, m_driftAngle / 10.0f, 0, Space.Self);
        rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(0, m_driftAngle / 10.0f, 0));
    }
    
    // Audio
    public void UpdateSound(int dir) 
    {
        drivingAudio.SetParameter("Driving", m_currentSpeed * dir);
    }

    public void ApplyMultiplier(float timer, float multiplier)
    {
        // If the speed modifier is already modified, determine's whether the effects should cancel each other, or overwrite
        // If the multiplier and speed modifier are opposite, cancel the effect
        // If the multiplier and speed modifier are the same, take the max/min in that dimension
        // Stops the coroutine timer if there was already one running and resets it
        if (m_speedModifier != 1.0f)
        {
            if((multiplier < 1.0f && m_speedModifier > 1.0f) || (multiplier > 1.0f && m_speedModifier < 1.0f))
            {
                m_speedModifier = 1.0f;
                return;
            } else if (m_speedModifier > 1.0f)
            {
                m_speedModifier = Math.Max(m_speedModifier, multiplier);
            } else
            {
                m_speedModifier = Math.Min(m_speedModifier, multiplier);
            }

        } else
        {
            m_speedModifier = multiplier;
        }

        if (m_resetTimer != null)
            StopCoroutine(m_resetTimer);
        
        StartCoroutine(ResetTimer(timer));
    }

    // Resets the speed modifier with a given timer
    public IEnumerator ResetTimer(float timer)
    {
        yield return new WaitForSeconds(timer);
        m_speedModifier = 1.0f;
    }
}
