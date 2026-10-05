using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteAlways]
public class RaceManager : MonoBehaviour
{
    [Header("Level")]
    [SerializeField]
    private StartFinishCheckpoint startFinishCheckpoint;
    
    private List<TrackCheckpoint> trackCheckpoints = new ();
    
    [SerializeField]
    private List<StartingGridSpot> startingGrid;

    [Header("Race Info")]
    [SerializeField]
    private List<RacerSelection> racerSelections;
    
    private List<RacerState> racerStates = new();
    
    public Action OnRaceStart;

    [SerializeField] public int maxLaps = 3;

    [SerializeField] public int countdownTimer = 3;  

    // TODO: This is just a temporary count of how many racers should the manager spawn. The real count is the # of racers in _racerStates
    public int racerCount = 1;
    
    public static RaceManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        StoreCheckpoints();
    }
    
#if UNITY_EDITOR
    private void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;
        StoreCheckpoints();
    }

    private void OnDisable()
    {
        if (Instance == this) Instance = null;
    }
#endif
    
    public void StartRace()
    {     
        if (racerSelections.Count == 0) return;

        // TODO: Mike - This only spawns 1 drive script for a singleplayer game. Change this for multiplayer!
        List<StartingGridSpot> remainingStartingGrid = startingGrid;
        int startingGridIndex = Random.Range(0, remainingStartingGrid.Count);
        StartingGridSpot startingGridSpot = startingGrid[startingGridIndex];
        remainingStartingGrid.RemoveAt(startingGridIndex);

        RacerSelection localRacerSelection = racerSelections[0]; // TODO: Currently, the local player selection is the first in the list
        // TODO: Going to use the first player as the UI timer. This works, but not the best practice
        (Character localCharacter, Kart localKart, RacerState localRacerState) = SpawnRacer(localRacerSelection, startingGridSpot, true);
        PlayerController localRacer = GameManager.Instance.LocalRacer;
        localRacer.AssignCharacterAndKart(localCharacter, localKart);
        localRacer.AssignRacerState(localRacerState);
        
        // TODO: Change this when we swap to multiplayer
        print("set immobile");
        localRacer.immobileDuration = countdownTimer;
        localRacer.preventMovement = true;
        
        List<RacerController> racerControllers = GameManager.Instance.GetRacers();
        racerControllers.Remove(localRacer);
        int racerSelectionIndex = 0;

        List<Item> items = GameManager.Instance.items;
        
        foreach (RacerController racer in racerControllers)
        {
            startingGridIndex = Random.Range(0, remainingStartingGrid.Count);
            startingGridSpot = startingGrid[startingGridIndex];
            remainingStartingGrid.RemoveAt(startingGridIndex);
            
            (Character character, Kart kart, RacerState racerState) = SpawnRacer(racerSelections[racerSelectionIndex], startingGridSpot, false);
            racer.AssignCharacterAndKart(character, kart);
            racer.AssignRacerState(racerState);
            
            // TODO: this works, but not for the Ais. 
            print("set immobile");
            racer.immobileDuration = countdownTimer;
            racer.preventMovement = true;
            
            
            racerSelectionIndex++;
        }

        // foreach (Item item in items)
        // {
        //     item.spawnItem();
        //     //Transform transform = item.itemObject.GetComponent<Transform>();
        //     int xCoord = 40 + ((int) (Random.value * 6) * 5);
        //     int zCoord = -100 - ((int) (Random.value * 6) * 5);
        //     transform.Translate(xCoord, 0, zCoord);
        // }
        
        OnRaceStart?.Invoke();
        GameManager.Instance.UIManager.StartCoroutine("TickCountdown", countdownTimer);

    }

    private (Character, Kart, RacerState) SpawnRacer(RacerSelection racerSelection, StartingGridSpot startingGridSpot, bool isPlayer)
    {
        Character character = Instantiate(racerSelection.character);
        Kart kart = Instantiate(racerSelection.kart);
        
        // Initialize kart + character.
        GameObject kartObject = Instantiate(kart.kartPrefab, startingGridSpot.GetSpawnPoint(), kart.kartPrefab.transform.rotation);
        GameObject characterSlot = kartObject.transform.Find("Model").Find("CharacterSocket").gameObject; // Finds Kart Model and Character Socket to Place Character
        GameObject characterObject = Instantiate(character.characterPrefab, Vector3.zero, character.characterPrefab.transform.rotation); // TODO: Mike - The rotation of the character is currently just based on the prefab. 
        characterObject.transform.SetParent(characterSlot.transform, false);
        character.characterObject = characterObject;
        kart.kartObject = kartObject;
        
        // Setup racer state.
        GameObject racerObject = new GameObject();
        RacerState racerState = racerObject.AddComponent<RacerState>();
        racerState.StartRace(startFinishCheckpoint);
        
        // Assign racer state to character.
        racerStates.Add(racerState);

        kartObject.tag = isPlayer ? "Player" : "AI";
        
        return (character, kart, racerState);
    }

    public void StoreCheckpoints()
    {
        if (startFinishCheckpoint == null) return;
        
        trackCheckpoints.Clear();
        trackCheckpoints.Add(startFinishCheckpoint);
        TrackCheckpoint currCheckpoint = startFinishCheckpoint.nextCheckpoint;

        int checkpointCounter = 0;
        startFinishCheckpoint.CheckpointID = checkpointCounter;
        while (currCheckpoint != startFinishCheckpoint)
        {
            trackCheckpoints.Add(currCheckpoint);
            currCheckpoint.CheckpointID = ++checkpointCounter;
            currCheckpoint = currCheckpoint.nextCheckpoint;
        }
    }

    public TrackCheckpoint GetCheckpoint(int id)
    {
        if (id < 0 || trackCheckpoints.Count <= id) return null;
        
        return trackCheckpoints[id];
    }
}