using Unity.Behavior;

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
        agent.Graph = RaceChoreographer.Instance.graph;

        agent.SetVariableValue("Racer", this);
        agent.SetVariableValue("CurrentWaypoint", RaceChoreographer.Instance.network.head);
    }

    // TODO: This will work for now
    public void PassInputs(float steering, float throttle, float brake)
    {
        Drive.Turn(steering);
        Drive.Accelerate(throttle);
    }
}
