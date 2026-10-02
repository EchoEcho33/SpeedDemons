using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/RacerRespawn")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "RacerRespawn", message: "[Racer] has respawned at [Checkpoint]", category: "Events", id: "5b154705ca471e28584dc2aedbd81dfc")]
public sealed partial class RacerRespawn : EventChannel<AIController, TrackCheckpoint> { }

