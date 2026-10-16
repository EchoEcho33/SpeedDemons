using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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

    private void Start()
    {
        // The function below is broken and doubles the amount of players.
        // GameManager.Instance.SetGameState(GameState.WaitingToStart);
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
        // this is the game's original code prior to the multiplayer split. depends on _singlePlayer
        if (GameManager.Instance._singlePlayer)
        {
            if (racerSelections.Count == 0) return;

        // TODO: Mike - This only spawns 1 drive script for a singleplayer game. Change this for multiplayer!
        List<StartingGridSpot> remainingStartingGrid = startingGrid;
        int startingGridIndex = Random.Range(0, remainingStartingGrid.Count);
        StartingGridSpot startingGridSpot = startingGrid[startingGridIndex];
        remainingStartingGrid.RemoveAt(startingGridIndex);

        RacerSelection localRacerSelection = GameManager.Instance.Player1; // TODO: Currently, the local player selection is the first in the list
        if (localRacerSelection.kart == null || localRacerSelection.character == null)
        {
            Debug.LogError("Player 1 Character Selection not found.");
            localRacerSelection = racerSelections[0];
        }
        (Character localCharacter, Kart localKart, RacerState localRacerState) = SpawnRacer(localRacerSelection, startingGridSpot, true);
        PlayerController localRacer = GameManager.Instance.LocalRacers[0];
        localRacer.AssignCharacterAndKart(localCharacter, localKart);
        localRacer.InitializeSingleCamera();
        localRacer.AssignRacerState(localRacerState);
        
        // TODO: Change this when we swap to multiplayer
        // ai countdown timer is handled by the racerController loop
        localRacer.immobileDuration = countdownTimer;
        localRacer.preventMovement = true;
        
        List<RacerController> racerControllers = GameManager.Instance.GetRacers();
        racerControllers.Remove(localRacer);
        int racerSelectionIndex = 0;

        List<Item> items = GameManager.Instance.items;
        
        GameManager.Instance.UIManager.StartCoroutine("TickCountdown", countdownTimer);
        
        foreach (RacerController racer in racerControllers)
        {
            startingGridIndex = Random.Range(0, remainingStartingGrid.Count);
            startingGridSpot = startingGrid[startingGridIndex];
            remainingStartingGrid.RemoveAt(startingGridIndex);
            
            (Character character, Kart kart, RacerState racerState) = SpawnRacer(racerSelections[racerSelectionIndex], startingGridSpot, false);
            racer.AssignCharacterAndKart(character, kart);
            racer.AssignRacerState(racerState);
            
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
        }
        else
        {
            if (racerSelections.Count == 0) return;
            
            List<StartingGridSpot> remainingStartingGrid = startingGrid;
            int startingGridIndex = Random.Range(0, remainingStartingGrid.Count);
            // Gianluca - DELETE THIS IMMEDIATELY. it works tho
            int P2startingGridIndex = Random.Range(0, remainingStartingGrid.Count);
            if (P2startingGridIndex == startingGridIndex)
            {
                P2startingGridIndex = Random.Range(0, remainingStartingGrid.Count);
            }
            StartingGridSpot startingGridSpot = startingGrid[startingGridIndex];
            StartingGridSpot P2startingGridSpot = startingGrid[P2startingGridIndex];
            
            remainingStartingGrid.RemoveAt(startingGridIndex);
            remainingStartingGrid.RemoveAt(P2startingGridIndex);

            // TODO: There is no multi character select right now, so both players get the same character. 
            RacerSelection localRacerSelection = GameManager.Instance.Player1; // TODO: Currently, the local player selection is the first in the list
            if (localRacerSelection.kart == null || localRacerSelection.character == null)
            {
                Debug.LogError("Player 1 Character Selection not found.");
                localRacerSelection = racerSelections[0];
            }
            (Character localCharacter, Kart localKart, RacerState localRacerState) = SpawnRacer(localRacerSelection, startingGridSpot, true);
            PlayerController player1 = GameManager.Instance.LocalRacers[0];
            player1.AssignCharacterAndKart(localCharacter, localKart);
            player1.InitializeMultiCamera(0);
            player1.AssignRacerState(localRacerState);
            
            // TODO: Change this to a loop when we swap to multiplayer more than 2
            // ai countdown timer is handled by the racerController loop
            player1.immobileDuration = countdownTimer;
            player1.preventMovement = true;
            
            (Character localCharacter2, Kart localKart2, RacerState localRacerState2) = SpawnRacer(localRacerSelection, P2startingGridSpot, true);
            
            PlayerController player2 = GameManager.Instance.LocalRacers[1];
            player2.AssignCharacterAndKart(localCharacter2, localKart2);
            // Check the script, but this is currently hardcoded for 2 players. 
            player2.InitializeMultiCamera(1);
            player2.AssignRacerState(localRacerState2);
            
            // TODO: Change this to a loop when we swap to multiplayer more than 2
            // ai countdown timer is handled by the racerController loop
            player2.immobileDuration = countdownTimer;
            player2.preventMovement = true;
            
            List<RacerController> racerControllers = GameManager.Instance.GetRacers();
            racerControllers.Remove(player1);
            racerControllers.Remove(player2);
            int racerSelectionIndex = 0;

            List<Item> items = GameManager.Instance.items;
            
            Debug.Log("tick countdown should've been called");
            GameManager.Instance.UIManager.StartCoroutine("TickCountdown", countdownTimer);
            
            foreach (RacerController racer in racerControllers)
            {
                startingGridIndex = Random.Range(0, remainingStartingGrid.Count);
                startingGridSpot = startingGrid[startingGridIndex];
                remainingStartingGrid.RemoveAt(startingGridIndex);
                
                (Character character, Kart kart, RacerState racerState) = SpawnRacer(racerSelections[racerSelectionIndex], startingGridSpot, false);
                racer.AssignCharacterAndKart(character, kart);
                racer.AssignRacerState(racerState);
                
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
        }
       
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