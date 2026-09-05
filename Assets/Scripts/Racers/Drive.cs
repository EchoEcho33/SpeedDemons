using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Drive : MonoBehaviour
{
    [SerializeField]
    private Character character;

    private Kart kart;

    private float m_currentSpeed = 0.0f;
    private float m_turnSpeed = 0.0f;

    private InputActionReference accelerate;

    private InputActionReference brake;

    private InputActionReference turn;

    public void Start()
    {
        kart = character.GetKart();
        accelerate = GameManager.Instance.input.accelerate;
        brake = GameManager.Instance.input.brake;
        turn = GameManager.Instance.input.turn;
    }

    public void Update()
    {

        if (accelerate.action.IsPressed())
        {
            Accelerate(accelerate.action.GetControlMagnitude());
        }
        else if (brake.action.IsPressed())
        {
            if (m_currentSpeed <= 0)
            {
                Accelerate(-brake.action.GetControlMagnitude() / 2);
            } else
                Decelerate(brake.action.GetControlMagnitude(), kart._brakeStrength);
        } else if (m_currentSpeed > 0)
        {
            Decelerate(1.0f, kart._drag);
        } else if (m_currentSpeed < 0)
        {
            Accelerate(0.5f);
        }

        Turn();
    }

    public Character getCharacter()
    {
        return character;
    }

    public void Accelerate(float amplitude)
    {
        float initialFrameSpeed = m_currentSpeed;
        m_currentSpeed += kart._maxAcceleration * amplitude * Time.deltaTime;

        if (m_currentSpeed > kart._maxSpeed)
            m_currentSpeed = kart._maxSpeed;

        if (m_currentSpeed < -kart._maxSpeed / 2)
            m_currentSpeed = -kart._maxSpeed / 2;

        float velocityUpdate = (m_currentSpeed + initialFrameSpeed) / 2 * Time.deltaTime;
        

        transform.position += transform.forward * velocityUpdate;
    }

    private void Decelerate(float amplitude, float a)
    {
        float initialFrameSpeed = m_currentSpeed;
        m_currentSpeed += -a * amplitude * Time.deltaTime;

        if (m_currentSpeed < 0)
            m_currentSpeed = 0;

        float velocityUpdate = (m_currentSpeed + initialFrameSpeed) / 2 * Time.deltaTime;

        transform.position += transform.forward * velocityUpdate;
    }

    private void Turn()
    {
        Vector2 turnDirection = turn.action.ReadValue<Vector2>();

        if (turnDirection.x != 0)
        {
            transform.position += Vector3.forward * kart._turnRadius * turnDirection.x * m_currentSpeed * Time.deltaTime / 100;
            transform.Rotate(0, kart._turnRadius * turnDirection.x * m_currentSpeed * Time.deltaTime, 0, Space.Self);
        } 

        // x = horizontal (- left + right) y = vertical (- down + up)
        
    }

}
