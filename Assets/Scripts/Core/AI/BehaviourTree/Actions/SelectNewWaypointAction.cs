using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SelectNewWaypoint", story: "[Racer] selects new [waypoint]", category: "Action", id: "7249da1b9552465ec5ca933907c3bd2f")]
public partial class SelectNewWaypointAction : Action
{
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
        Waypoint.Value = waypoint == null ? RaceChoreographer.Instance.network.head : waypoint.Next[0]; // TODO: Swap this out when its not just a telemetry recording
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

