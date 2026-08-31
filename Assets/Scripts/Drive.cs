using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Drive : MonoBehaviour
{
    [SerializeField]
    private float _maxSpeed = 25.0f;

    [SerializeField]
    private float _maxAcceleration = 10.0f;

    [SerializeField]
    private float _brakeStrength = 10.0f;

    [SerializeField]
    private float _drag = 4.0f;

    [SerializeField, Range(0, 75)]
    private int _turnRadius;

    [SerializeField]
    private float _traction;

    private float m_currentSpeed = 0.0f;
    private float m_turnSpeed = 0.0f;

    [SerializeField]
    public InputActionReference accelerate;

    [SerializeField]
    public InputActionReference brake;

    [SerializeField]
    public InputActionReference turn;

    [SerializeField]
    public InputActionReference reverse;

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
                Decelerate(brake.action.GetControlMagnitude(), _brakeStrength);
        } else if (m_currentSpeed > 0)
        {
            Decelerate(1.0f, _drag);
        } else if (m_currentSpeed < 0)
        {
            Accelerate(0.5f);
        }

        Turn();
    }

    public void Accelerate(float amplitude)
    {
        float initialFrameSpeed = m_currentSpeed;
        m_currentSpeed += _maxAcceleration * amplitude * Time.deltaTime;

        if (m_currentSpeed > _maxSpeed)
            m_currentSpeed = _maxSpeed;

        if (m_currentSpeed < -_maxSpeed / 2)
            m_currentSpeed = -_maxSpeed / 2;

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
            transform.position += Vector3.forward * _turnRadius * turnDirection.x * m_currentSpeed * Time.deltaTime / 100;
            transform.Rotate(0, _turnRadius * turnDirection.x * m_currentSpeed * Time.deltaTime, 0, Space.Self);
        } 

        // x = horizontal (- left + right) y = vertical (- down + up)
        
    }

}
