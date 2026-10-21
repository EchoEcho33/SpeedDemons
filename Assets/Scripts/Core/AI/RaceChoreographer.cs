using Unity.Behavior;
using UnityEngine;

public class RaceChoreographer : MonoBehaviour
{
    public BehaviorGraph graph;

    [SerializeField]
    public NavigationNetwork network;
    
    [SerializeField]
    private RacerRespawn racerRespawnEvent;
    
    private int nextRacingLine;

    public static RaceChoreographer Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AssignRacingLine(AIController racer)
    {
        // Round robin, so racers are spread evenly across the racing lines.
        racer.AssignRacingLine(network.RacingLineCount > 0 ? nextRacingLine++ % network.RacingLineCount : -1);
        Debug.Log($"{racer.name} assigned to racing line {racer.RacingLineIndex}.");
    }

    public void SendRacerRespawnEvent(AIController racer, TrackCheckpoint checkpoint)
    {
        racerRespawnEvent.SendEventMessage(racer, checkpoint);
    }
}
