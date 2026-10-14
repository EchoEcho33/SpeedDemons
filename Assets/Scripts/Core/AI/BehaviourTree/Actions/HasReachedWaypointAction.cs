using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "HasReachedWaypoint", story: "[ReachedDestination] = Has the [racer] reached the [waypoint]", category: "Action", id: "6c312d6f07ff85e88a0b064a18198d69")]
public partial class HasReachedWaypointAction : Action
{
    [SerializeReference] public BlackboardVariable<bool> ReachedDestination;
    [SerializeReference] public BlackboardVariable<AIController> Racer;
    [SerializeReference] public BlackboardVariable<NavigationWaypoint> Waypoint;
    private AIController racer;
    private NavigationWaypoint waypoint;
    
    protected override Status OnStart()
    {
        racer = Racer.Value;
        waypoint = Waypoint.Value;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Vector3 kartPos = racer.GetKartPosition();  
        Vector3 waypointPos = waypoint.position;

        if (Vector2.Distance(waypointPos, kartPos) < 2f)
        {
            ReachedDestination.Value = true;
        }
        
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

