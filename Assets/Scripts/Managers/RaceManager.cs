
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    [SerializeField]
    private List<Character> racers;
    private List<RacerState> _racerStates = new();
    [SerializeField]
    private StartFinishCheckpoint _startFinishCheckpoint;
    
    public void Start()
    {
        // TODO: Mike - I'm just doing this for now for testing purposes. Move this later!
        StartRace();
    }
    
    public void StartRace()
    {
        Character[] racers = FindObjectsByType<Character>(FindObjectsSortMode.None);
        
        foreach (Character racer in racers)
        {
            GameObject racerGO = new GameObject();
            RacerState racerState = racerGO.AddComponent<RacerState>();
            
            racerState.StartRace(_startFinishCheckpoint);
            _racerStates.Add(racerState);
            racer.AssignRacerState(racerState);
        }
    }
}