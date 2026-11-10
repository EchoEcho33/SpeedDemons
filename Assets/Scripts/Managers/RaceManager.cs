using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
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
    [SerializeField] private float itemRespawnTime = 10.0f;

    // TODO: This is just a temporary count of how many racers should the manager spawn. The real count is the # of racers in _racerStates
    public int racerCount = 1;
    
    private List<RacerState> finishedRacers = new();
    
    private List<RacerState> finishedPlayerRacers = new();
    
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
        /*if (GameManager.Instance._singlePlayer)
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
        localRacer.input.enabled = true; 
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
        }*/
        
        if (racerSelections.Count == 0) return;
        List<RacerController> racerControllers = GameManager.Instance.GetRacers();

        List<StartingGridSpot> remainingStartingGrid = startingGrid;
        int[] excluded = new int[startingGrid.Count];
        
        for (int i = 0; i < GameManager.Instance.numPlayers; i++)
        {
            int startingGridIndex = RandomExceptList(0, startingGrid.Count, excluded);
            // excluded.Add(startingGridIndex);
            
            // DELETE THIS
            StartingGridSpot startingGridSpot = startingGrid[i];
            
            remainingStartingGrid.Remove(startingGridSpot);
            
            // TODO: There is no multi character select right now, so all players get the same character. 
            RacerSelection localRacerSelection = GameManager.Instance.Player1; // TODO: Currently, the local player selection is the first in the list
            if (localRacerSelection.kart == null || localRacerSelection.character == null)
            {
                Debug.LogError("Player" + i + "Character Selection not found.");
                localRacerSelection = racerSelections[0];
            }
            (Character localCharacter, Kart localKart, RacerState localRacerState) = SpawnRacer(localRacerSelection, startingGridSpot, true);
            PlayerController player = GameManager.Instance.LocalRacers[i];
            player.AssignCharacterAndKart(localCharacter, localKart);
            player.InitializeSingleCamera();
            
            
            player.AssignRacerState(localRacerState);
            
            // TODO: Change this to a loop when we swap to multiplayer more than 2
            // ai countdown timer is handled by the racerController loop
            player.immobileDuration = countdownTimer;
            player.preventMovement = true;

            
            racerControllers.Remove(player);
        }
            
        int racerSelectionIndex = 0;

        List<Item> items = GameManager.Instance.items;
            
        GameManager.Instance.UIManager.StartCoroutine("TickCountdown", countdownTimer);
            
        /*foreach (RacerController racer in racerControllers)
        {
            // int startingGridIndex = random_except_list(startingGrid.Count, excluded);
            // excluded.Add(startingGridIndex);
            
            // FIX
            StartingGridSpot startingGridSpot = startingGrid[0];
                
                
            (Character character, Kart kart, RacerState racerState) = SpawnRacer(racerSelections[racerSelectionIndex], startingGridSpot, false);
            racer.AssignCharacterAndKart(character, kart);
            racer.AssignRacerState(racerState);
                
            racer.immobileDuration = countdownTimer;
            racer.preventMovement = true;
                
            racerSelectionIndex++;
        }*/
        // foreach (Item item in items)
        // {
        //     item.spawnItem();
        //     //Transform transform = item.itemObject.GetComponent<Transform>();
        //     int xCoord = 40 + ((int) (Random.value * 6) * 5);
        //     int zCoord = -100 - ((int) (Random.value * 6) * 5);
        //     transform.Translate(xCoord, 0, zCoord);
        // }
        
        OnRaceStart?.Invoke();
            
        // DO NOT modify the splitscreen setting in the editor.
        // Must ensure that the splitscreen handler is called after initializing all the cameras.
        // - Gianluca
        GameManager.Instance.inputManagerPrefab.GetComponent<PlayerInputManager>().splitScreen = true;
        
    }

    private (Character, Kart, RacerState) SpawnRacer(RacerSelection racerSelection, StartingGridSpot startingGridSpot, bool isPlayer)
    {
        Character character = Instantiate(racerSelection.character);
        Kart kart = Instantiate(racerSelection.kart);
        
        // Initialize kart + character.
        GameObject kartObject = Instantiate(kart.kartPrefab, startingGridSpot.GetSpawnPoint(), kart.kartPrefab.transform.rotation);
        if (!isPlayer)
        {
            Destroy(kartObject.GetComponent<PlayerInput>());
        }
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

    public void FinishRace(RacerState racerState)
    {
        finishedRacers.Add(racerState);

        if (racerState.RacerController is PlayerController playerController)
        {
            finishedPlayerRacers.Add(racerState);

            if (finishedPlayerRacers.Count >= GameManager.Instance.numPlayers)
            {
                FinishRaceTimer();
            }
        }
    }

    private async void FinishRaceTimer()
    {
        for (int seconds = 10; seconds > 0; seconds--)
        {
            GameManager.Instance.UIManager.TickTimer(seconds);
            await Awaitable.WaitForSecondsAsync(1.0f, destroyCancellationToken);
        }
        GameManager.Instance.SetGameState(GameState.FinishedMatch);
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

    public void RespawnSelf(GameObject gameObject)
    {
        StartCoroutine(RespawnTimer(gameObject));
    }

    private IEnumerator<WaitForSeconds> RespawnTimer(GameObject gameObject)
    {
        yield return new WaitForSeconds(itemRespawnTime);
        gameObject.SetActive(true);
    }

    // lowkey copied this from stack overflow
    // - gianluca

    public static int RandomExceptList(int min, int max, int[] forbiddenElements) 
    {
        bool numberAllowed = true;
        
        if (forbiddenElements.Length>=1)
        {
            //this part here checks indefinitely for an allowed randomly generated number (outside "forbiddenElements" array). Eventually it will find it and return it and therefore terminate the loop.
            while(true)
            {
                numberAllowed=true;
            
                int r = Random.Range(min,max+1);
        
                for(int i=0;i<forbiddenElements.Length;i++)
                {
                    if (r==forbiddenElements[i])
                        numberAllowed=false;
                }
                if(numberAllowed)
                    return r;
            }
        }
        else
        {
            int r = Random.Range(min,max+1);
            return r;
        }
    }
    

}
