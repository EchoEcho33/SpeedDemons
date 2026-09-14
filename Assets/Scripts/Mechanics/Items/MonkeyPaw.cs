using System.Data;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class MonkeyPaw : Item
{

    [Header("Monkey's Paw Attributes")]
    //placeholder/test values
    public float boost = 2.0f;
    public float decel = 0.7f;
    public float duration_up = 2;
    public float duration_down = 2;

    //time to decel into decel state
    public float timedecel = 0.5f;

    private float timeRemaining = 0;
    private float decel_calc = 0;

    //temp connection, dont know which class to use really
    private RacerController user;
    private float baseAccel = 0;
    private float baseSpeed = 0;

    private enum State
    {
        BOOST,
        DECEL,
        INACTIVE
    }

    private State state = State.INACTIVE;

    public override void Use()
    {
        if (state != State.INACTIVE) { return; }

        //temp for test
        user = FindFirstObjectByType<PlayerController>();
        baseAccel = user.Kart._maxAcceleration;
        baseSpeed = user.Kart._maxSpeed;

        state = State.BOOST;
        timeRemaining = duration_up;
        user.Kart._maxAcceleration = baseAccel * boost;
        user.Kart._maxSpeed = baseSpeed * boost;

        decel_calc = ((decel - boost) * baseSpeed) / timedecel;
    }

    public void Update()
    {
        
        if (state == State.INACTIVE || user == null) return;
        if (timeRemaining > -duration_down)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining < 0)
            {
                if (state == State.BOOST)
                {
                    user.Kart._maxAcceleration = decel_calc;
                    state = State.DECEL;
                }
              
                if (user.Kart._maxAcceleration == decel_calc && user.Drive.m_currentSpeed <= baseSpeed * decel)
                {
                    user.Kart._maxAcceleration = baseAccel * decel;
                    user.Kart._maxSpeed = baseSpeed * decel;

                }


            }
        }
        else
        {
            user.Kart._maxAcceleration = baseAccel;
            user.Kart._maxSpeed = baseSpeed;
            state = State.INACTIVE;
        }
    }

    public void OnDestroy()
    {
        if (user == null) return;
        user.Kart._maxAcceleration = baseAccel;
        user.Kart._maxSpeed = baseSpeed;
    }
}
