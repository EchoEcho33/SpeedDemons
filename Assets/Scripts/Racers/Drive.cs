using UnityEngine;

public class Drive : MonoBehaviour
{
    public RacerController Racer { get; private set; }
    
    private Character character;

    private Kart kart;

    private float m_currentSpeed = 0.0f;
    private float m_turnSpeed = 0.0f;

    public void AssignController(RacerController newRacerController)
    {
        Racer = newRacerController;
    }

    public void Initialize(Character newCharacter, Kart newKart)
    {
        character = newCharacter;
        kart = newKart;
    }

    public void Accelerate(float amplitude)
    {
        if (!IsGrounded()) return;
        
        float initialFrameSpeed = m_currentSpeed;
        m_currentSpeed += kart._maxAcceleration * amplitude * Time.deltaTime;

        if (m_currentSpeed > kart._maxSpeed)
            m_currentSpeed = kart._maxSpeed;

        if (m_currentSpeed < -kart._maxSpeed / 2)
            m_currentSpeed = -kart._maxSpeed / 2;

        float velocityUpdate = (m_currentSpeed + initialFrameSpeed) / 2 * Time.deltaTime;
        

        transform.position += transform.forward * velocityUpdate;
    }

    public void Decelerate(float amplitude, float a)
    {
        float initialFrameSpeed = m_currentSpeed;
        m_currentSpeed += -a * amplitude * Time.deltaTime;

        if (m_currentSpeed < 0)
            m_currentSpeed = 0;

        float velocityUpdate = (m_currentSpeed + initialFrameSpeed) / 2 * Time.deltaTime;

        transform.position += transform.forward * velocityUpdate;
    }

    public void Turn(float turnDirection)
    {
        if (IsGrounded())
        {
            if (turnDirection != 0)
            {
                transform.position += Vector3.forward * kart._turnRadius * turnDirection * m_currentSpeed * Time.deltaTime / 100;
                transform.Rotate(0, kart._turnRadius * turnDirection * m_currentSpeed * Time.deltaTime, 0, Space.Self);
            } 
        }

        // x = horizontal (- left + right) y = vertical (- down + up)
        
    }

    // TODO: Check that wheels are grounded
    private bool IsGrounded()
    {
        return true;
    }

    public float GetCurrentSpeed()
    {
        return m_currentSpeed;
    }
}
