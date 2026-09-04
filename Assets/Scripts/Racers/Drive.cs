using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Drive : MonoBehaviour
{
    private float _maxSpeed = 25.0f;

    private float _maxAcceleration = 10.0f;

    private float _brakeStrength = 10.0f;

    private float _drag = 4.0f;

    private int _turnRadius = 10;

    private float _traction = 0;

    private float m_currentSpeed = 0.0f;
    private float m_turnSpeed = 0.0f;

    private InputActionReference accelerate;

    private InputActionReference brake;

    private InputActionReference turn;

    public void Init(float maxSpeed, float maxAcceleration, float brakeStrength, float drag, int turnRadius, float traction)
    {
        _maxSpeed = maxSpeed;
        _maxAcceleration = maxAcceleration;
        _brakeStrength = brakeStrength;
        _drag = drag;
        _turnRadius = turnRadius;
        _traction = traction;

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
