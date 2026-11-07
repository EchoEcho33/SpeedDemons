using System;
using System.Collections;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;

public class Drive : MonoBehaviour
{
    public RacerController Racer { get; private set; }
    
    public Character character { get; private set; }

    private Kart kart;

    public float m_currentSpeed = 0.0f;
    private float m_speedModifier = 1.0f;
    private Coroutine m_resetTimer;
    // private float m_turnSpeed = 0.0f;
    // private bool m_isDrifting = false;
    private int m_driftAngle = 0;
    public StudioEventEmitter drivingAudio;
    private Rigidbody rigidbody;

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
    }

    public void Accelerate(float amplitude)
    {
        if (!IsGrounded()) return;
        
        float initialFrameSpeed = m_currentSpeed;
        m_currentSpeed += kart._maxAcceleration * amplitude * Time.deltaTime * m_speedModifier;

        if (m_currentSpeed > kart._maxSpeed * m_speedModifier)
        {
            m_currentSpeed = kart._maxSpeed * m_speedModifier;
            m_currentSpeed -= Time.deltaTime * m_speedModifier * kart._maxAcceleration;
            if (m_currentSpeed < kart._maxSpeed)
            {
                m_currentSpeed = kart._maxSpeed * m_speedModifier;
            }
        }

        if (m_currentSpeed < -kart._maxSpeed * m_speedModifier / 2)
            m_currentSpeed = -kart._maxSpeed * m_speedModifier / 2;

        float velocityUpdate = (m_currentSpeed + initialFrameSpeed) / 2 * Time.deltaTime * m_speedModifier;
        

        transform.position += transform.forward * velocityUpdate;
        UpdateSound(1);
    }

    public void Decelerate(float amplitude, float a)
    {
        float initialFrameSpeed = m_currentSpeed;
        m_currentSpeed += -a * amplitude * Time.deltaTime * m_speedModifier;

        if (m_currentSpeed < 0)
            m_currentSpeed = 0;

        float velocityUpdate = (m_currentSpeed + initialFrameSpeed) / 2 * Time.deltaTime * m_speedModifier;

        transform.position += transform.forward * velocityUpdate;
        UpdateSound(-1);
    }

    public void Turn(float turnDirection, float turnMultiplier = 1.0f)
    {
        if (IsGrounded())
        {
            if (m_driftAngle != 0)
            {
                OrientTowardsVelocity();
            }
            
            if (turnDirection != 0)
            {
                transform.position += Vector3.forward * kart._turnRadius * turnDirection * turnMultiplier * m_currentSpeed * Time.deltaTime / 100;
                //rigidbody.linearVelocity = kart._turnRadius * turnDirection * turnMultiplier * rigidbody.linearVelocity * Time.deltaTime / 100;

                transform.Rotate(0, kart._turnRadius * turnDirection * turnMultiplier * m_currentSpeed * Time.deltaTime, 0, Space.Self);
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
        }


        // x = horizontal (- left + right) y = vertical (- down + up)
        
    }

    public void SpinOut()
    {
        if (!IsGrounded()) return;

        float initialFrameSpeed = m_currentSpeed;
        float slowdownSpeed = 20;

        m_currentSpeed -= slowdownSpeed * Time.deltaTime;

        if (m_currentSpeed < 0)
            m_currentSpeed = 0;

        float velocityUpdate = (m_currentSpeed + initialFrameSpeed) / 2 * Time.deltaTime;

        transform.position += transform.forward * velocityUpdate;
    }

    public void PreventMovement()
    {
        if (!IsGrounded()) return;
        m_currentSpeed = 0;
    }

    // TODO: Check that wheels are grounded
    private bool IsGrounded()
    {
        Debug.Log("This is being called" + transform.position);
        Ray groundCheck = new Ray(transform.position + Vector3.up * 0.4f, Vector3.down);
        Debug.DrawRay(transform.position + Vector3.up * 0.4f, Vector3.down * 0.4f, Color.red, 0.25f);
        return true;
    }

    public float GetCurrentSpeed()
    {
        return m_currentSpeed;
    }

    private void OrientTowardsVelocity()
    {
        transform.Rotate(0, -m_driftAngle / 10.0f, 0, Space.Self);
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

        transform.Rotate(0, m_driftAngle / 10.0f, 0, Space.Self);
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

        transform.Rotate(0, m_driftAngle / 10.0f, 0, Space.Self);
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
