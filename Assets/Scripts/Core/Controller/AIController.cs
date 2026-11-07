using Unity.Behavior;
using UnityEngine;
using UnityEngine.Rendering;

public class AIController : RacerController
{
    public BehaviorGraphAgent agent { get; private set; }
    
    public override void InitializeDrive() 
    {
        Drive = Kart.kartObject.AddComponent<Drive>();
        Drive.AssignController(this);
        Drive.Initialize(Character, Kart);
        
        InitializeAI();
    }

    private void InitializeAI()
    {
        agent = gameObject.AddComponent<BehaviorGraphAgent>();
        if (!RaceChoreographer.Instance)
        {
            Debug.LogError("No RaceChoreographer found. AI disabled.");
            return;
        }

        if (!RaceChoreographer.Instance.network.head)
        {
            Debug.LogError("No navigation head found. AI disabled.");
            RaceChoreographer.Instance.network.OnNetworkGenerated += InitializeAIBehavior;
            return;
        }

        InitializeAIBehavior();
    }

    private void InitializeAIBehavior()
    {
        RaceChoreographer.Instance.network.OnNetworkGenerated -= InitializeAIBehavior;
        
        agent.Graph = RaceChoreographer.Instance.graph;

        agent.SetVariableValue("Racer", this);
        agent.SetVariableValue("CurrentWaypoint", RaceChoreographer.Instance.network.head);
    }

    // TODO: This will work for now
    public void PassInputs(float steering, float throttle, float brake)
    {
        // This check is probably temporary, but it is essentially the same as the Race Countdown handler in PlayerController.
        if (preventMovement)
        {
            Drive.PreventMovement();
            immobileDuration -= Time.deltaTime;
            
            if (base.immobileDuration <= 0)
            {
                base.preventMovement = false;
            }
        }
        else
        {
            Drive.Turn(steering);
            Drive.Accelerate(throttle);   
        }
    }
}
