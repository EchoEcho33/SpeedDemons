using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Drive", story: "[Racer] drives to [Waypoint]", category: "Action", id: "eb3906e890ecadcbb6572c112c838802")]
public partial class DriveAction : Action
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

    // TODO: We need to def improve this. But it works for M1.5
    protected override Status OnUpdate()
    {
        Vector3 kartPos = racer.GetKartPosition();
        Vector3 waypointPos = waypoint.position;
        
        if (Vector2.Distance(waypointPos, kartPos) < 0.1f) return Status.Success;
        
        Vector3 vectorToTarget = waypointPos - kartPos;
        vectorToTarget.Normalize();
        
        Vector3 forward = racer.GetCharacterAndKart().transform.forward;
        float angleToTarget = Vector3.SignedAngle(forward, vectorToTarget, Vector3.up);

        float steerAmount = angleToTarget / 30.0f;

        steerAmount = Mathf.Clamp(steerAmount, -1.0f, 1.0f);
        racer.PassInputs(steerAmount, 1.0f, 0.0f);
        
        return Status.Running;
    }

    protected override void OnEnd()
    {
        
    }
}

