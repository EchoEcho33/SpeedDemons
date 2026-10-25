using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SelectNewWaypointAtCheckpoint", story: "if [Racer] is [RespawnedRacer] , selects new [waypoint] at [checkpoint]", category: "Action", id: "c89e7b0c2dd7cbdf10746ea1eda96605")]
public partial class SelectNewWaypointAtCheckpointAction : Action
{
    [SerializeReference] public BlackboardVariable<AIController> Racer;
    [SerializeReference] public BlackboardVariable<AIController> RespawnedRacer;
    [SerializeReference] public BlackboardVariable<NavigationWaypoint> Waypoint;
    [SerializeReference] public BlackboardVariable<TrackCheckpoint> Checkpoint;
    
    private AIController racer;
    private AIController respawnedRacer;
    private NavigationWaypoint waypoint;
    private TrackCheckpoint checkpoint;
    
    protected override Status OnStart()
    {
        racer = Racer.Value;
        respawnedRacer = RespawnedRacer.Value;
        
        if (racer != respawnedRacer) return Status.Failure;
        
        checkpoint = Checkpoint.Value;

        int checkpointID = checkpoint.CheckpointID;
        NavigationWaypoint newWaypoint = RaceChoreographer.Instance.network.GetCheckpointWaypoint(checkpointID);
        Waypoint.Value = newWaypoint.Next[0];
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

