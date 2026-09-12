using System.Data;
using UnityEngine;

public class MonkeyPaw : Item
{
    //placeholder/test values
    public float boost = 2.0f;
    public float decel = 0.7f;
    public float duration_up = 2;
    public float duration_down = 2;

    //time to decel into decel state
    public float timedecel = 0.5f;

    private float timeRemaining = 0;
    private float decel_calc = 0;

    //temp idea, need a solid idea for connecting item to its user for purpose of who to use item on/not on
    protected Drive user;
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
        user = FindFirstObjectByType<Drive>();
        baseAccel = user.getCharacter().GetKart()._maxAcceleration;
        baseSpeed = user.getCharacter().GetKart()._maxSpeed;

        state = State.BOOST;
        timeRemaining = duration_up;
        user.getCharacter().GetKart()._maxAcceleration = baseAccel * boost;
        user.getCharacter().GetKart()._maxSpeed = baseSpeed * boost;

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
                    user.getCharacter().GetKart()._maxAcceleration = decel_calc;
                    state = State.DECEL;
                }
              
                if (user.getCharacter().GetKart()._maxAcceleration == decel_calc && user.m_currentSpeed <= baseSpeed * decel)
                {
                    user.getCharacter().GetKart()._maxAcceleration =baseAccel * decel;
                    user.getCharacter().GetKart()._maxSpeed = baseSpeed * decel;

                }


            }
        }
        else
        {
            user.getCharacter().GetKart()._maxAcceleration = baseAccel;
            user.getCharacter().GetKart()._maxSpeed = baseSpeed;
            state = State.INACTIVE;
        }
    }

    public void OnDestroy()
    {
        if (user == null) return;
        user.getCharacter().GetKart()._maxAcceleration = baseAccel;
        user.getCharacter().GetKart()._maxSpeed = baseSpeed;
    }
}
