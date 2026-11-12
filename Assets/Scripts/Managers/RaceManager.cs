using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using UnityEngine.XR;
using InputDevice = UnityEngine.InputSystem.InputDevice;
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

    [Header("Player UI")] [SerializeField] private GameObject PlayerUIPrefab; 
    
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
        // this is unsafe, but 
        
        
        if (racerSelections.Count == 0) return;
        List<RacerController> racerControllers = GameManager.Instance.GetRacers();

        // TODO: this isnt used anymore, could remove
        List<StartingGridSpot> remainingStartingGrid = startingGrid;
        List<int> excluded = new List<int>(); 
        
        for (int i = 0; i < GameManager.Instance.numPlayers; i++)
        {
            int startingGridIndex = RandomRangeExcept(0, startingGrid.Count, excluded);
            excluded.Add(startingGridIndex);
            
            StartingGridSpot startingGridSpot = startingGrid[i];
            
            remainingStartingGrid.Remove(startingGridSpot);
            
            // TODO: There is no multi character select right now, so all players get the same character. 
            RacerSelection localRacerSelection = racerSelections[i];
            if (localRacerSelection.kart == null || localRacerSelection.character == null)
            {
                Debug.LogError("Player" + i + "Character Selection not found.");
                localRacerSelection = racerSelections[0];
            }
            (Character localCharacter, Kart localKart, RacerState localRacerState) = SpawnRacer(localRacerSelection, startingGridSpot, true);
            
            PlayerController player = GameManager.Instance.LocalRacers[i];
            player.AssignCharacterAndKart(localCharacter, localKart);
            player.MainCamera = localKart.kartObject.GetComponent<PlayerInput>().camera;
            
            // per player ui setup
            player.playerUI = Instantiate(PlayerUIPrefab).GetComponent<RaceUI>();
            player.playerUI.playerCamera = player.MainCamera;
            player.playerUI.viewport.gameObject.SetActive(false);
            
            player.AssignRacerState(localRacerState);
            
            player.immobileDuration = countdownTimer;
            player.preventMovement = true;

            
            racerControllers.Remove(player);
            
        }
            
        // start after the local selections
        int racerSelectionIndex = GameManager.Instance.numPlayers;

        List<Item> items = GameManager.Instance.items;
            
        GameManager.Instance.UIManager.StartCoroutine("TickCountdown", countdownTimer);
            
        foreach (RacerController racer in racerControllers)
        {
            int startingGridIndex = RandomRangeExcept(0, startingGrid.Count, excluded);
            excluded.Add(startingGridIndex);
            
            // FIX
            StartingGridSpot startingGridSpot = startingGrid[startingGridIndex];
                
            // TODO: make a preset of ai characters, just so we don't 
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
        
        // Must ensure that the splitscreen handler is only created after initializing all the cameras. Otherwise, splitscreen breaks
        PlayerInputManager inputManager = Instantiate(GameManager.Instance.inputManagerPrefab).GetComponent<PlayerInputManager>();
        inputManager.splitScreen = true;

    }

    private (Character, Kart, RacerState) SpawnRacer(RacerSelection racerSelection, StartingGridSpot startingGridSpot, bool isPlayer)
    {
        Character character = Instantiate(racerSelection.character);
        Kart kart = Instantiate(racerSelection.kart);
        
        // Initialize kart + character.
        // The PlayerInputManager requires JoinPlayer to be used for splitscreen, so if this is a player, we instantiate via that method
        // - Gianluca
        // The racerselection should eventually contain a new field that holds the input preference of this player for 4 players. 
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
        
        // setup camera for players. effectively equal to InitializeCamera, which was a method that was only ever called once.
        // moved it here to prevent race conditions. - Gianluca
        if (isPlayer)
        {
            if (racerSelection.inputDevice != null)
            {
                InputUser.PerformPairingWithDevice(racerSelection.inputDevice, kartObject.GetComponent<PlayerInput>().user);
            }
            else
            {
                // we dont have any input selected on a player kart. maps the next available input to this new racer
                if (GameManager.Instance.JoinedInputDevices.Any())
                {
                    InputUser.PerformPairingWithDevice(GameManager.Instance.JoinedInputDevices.Dequeue(), kartObject.GetComponent<PlayerInput>().user);    
                }
                
            }
            //TODO: remove debug
            Debug.Log("Printing player's inputs");
            foreach (InputDevice input in kartObject.GetComponent<PlayerInput>().devices)
            {
                Debug.Log(input);
            }
            
            GameObject cameraObject = Instantiate(GameManager.Instance.cameraPrefab);
            Camera mainCamera = cameraObject.GetComponent<Camera>();
            kartObject.GetComponent<PlayerInput>().camera = mainCamera; 
            
            CinemachineBrain brain = mainCamera.GetComponent<CinemachineBrain>();
            
            // From outputchannels source:
            // channels are designated by an enum in powers of two, so if we cast it, it should increment correctly
            // - gianluca
    
            GameObject cinemachineCameraObject = Instantiate(GameManager.Instance.cinemachineCameraPrefab);
            CinemachineCamera cinemachine = cinemachineCameraObject.GetComponent<CinemachineCamera>();
            
            cinemachine.Follow = kart.kartObject.transform;

            // audio stuff
            mainCamera.GetComponent<StudioListener>().SetAttenuationObject(mainCamera.gameObject);
            
            brain.ChannelMask = indexChannel(GameManager.Instance.currentCameraChannel);
            cinemachine.OutputChannel = indexChannel(GameManager.Instance.currentCameraChannel);

            GameManager.Instance.currentCameraChannel++;
            
            
        }
        
        // Setup racer state.
        GameObject racerObject = new GameObject("RacerStateObject");
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

    // lowkey copied this from unity forums
    // - gianluca

    public static int RandomRangeExcept(int minInclusive, int maxExclusive, List<int> except = null)
    {
        IEnumerable<int> allowedRandomNumbers = Enumerable.Range(minInclusive, maxExclusive - minInclusive);
        if (except != null)
        {
            allowedRandomNumbers = allowedRandomNumbers.Except(except);
        }
        if (allowedRandomNumbers.Count() == 0)
        {
            Debug.LogError("Allowed numbers array is empty! Returning 0");
            return 0;
        }
        int randomIndex = Random.Range(0, allowedRandomNumbers.Count());
        return allowedRandomNumbers.ElementAt(randomIndex);
    }
    
    /*
     * The output channel is an enum and manually indexing was not working.
     * This is an ugly switch statement that takes our value and turns it into the appropriate channel.
     * There will only be 4 players so there's 4 cases.
     * idk why standard indexing doesn't work, but this does.
     * - Gianluca
     */
    private OutputChannels indexChannel(int index)
    {
        switch (index)
        {
            case 1:
                return OutputChannels.Channel01;
            case 2:
                return OutputChannels.Channel02;
            case 3:
                return OutputChannels.Channel03;
            case 4:
                return OutputChannels.Channel04;
            default:
                return OutputChannels.Default;
        }
    }

}
