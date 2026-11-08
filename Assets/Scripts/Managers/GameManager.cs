using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    Lobby,
    WaitingToStart,
    InMatch,
    FinishedMatch,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set;}

    public InputSys inputList { get; private set;}

    public RaceUI UIManager { get; private set;}
    
    public AudioManager audioManager { get; private set;}
    
    [SerializeField]
    public GameObject inputManagerPrefab;

    [SerializeField]
    public GameObject UIManagerPrefab;
    
    [SerializeField]
    public GameObject AudioManagerPrefab;
    
    [SerializeField] 
    public GameObject cameraPrefab;
        
    [SerializeField] 
    public GameObject cinemachineCameraPrefab;

    /// <summary>
    /// The default game state when the game manager is initialized.
    /// Should be set to WaitingToStart in track scenes, or MainMenu for the title screen.
    /// </summary>
    public GameState startingGameState = GameState.Lobby;
    
    // default to multiplayer
    [SerializeField] public int numPlayers = 1;

    public GameState GameState { get; private set; }

    public List<PlayerController> LocalRacers = new();

    public RacerSelection Player1 {get; private set;}
    public RacerSelection Player2 {get; private set;}
    
    private List<RacerController> _racers = new();
    public List<RacerController> Racers => new(_racers);
    
    [Header("Databases")]
    [SerializeField]
    public List<Item> items;

    [SerializeField]
    public List<Character> characters;

    [SerializeField]
    public List<Kart> karts;
    
    // for cinemachine brain channel setup
    public int currentCameraChannel = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // For testing purposes, the default value forces a match to start.
        SetGameState(startingGameState);
    }

    public void SetGameState(GameState newGameState)
    {
        GameState = newGameState;
        HandleGameStateSet();
    }

    private void HandleGameStateSet()
    {
        switch (GameState)
        {
            case GameState.MainMenu:
                HandleGameStateSetMainMenu();
                break;
            
            case GameState.Lobby:
                HandleGameStateSetLobby();
                break;
            
            case GameState.WaitingToStart:
                HandleGameStateSetWaitingToStart();
                break;
            
            case GameState.InMatch:
                HandleGameStateSetInMatch();
                break;
            
            case GameState.FinishedMatch:
                HandleGameStateSetFinishedMatch();
                break;
        }
    }

    private void HandleGameStateSetMainMenu()
    {
        
    }

    private void HandleGameStateSetLobby()
    {
        
    }

    private void HandleGameStateSetWaitingToStart()
    {
        InitializeManagers();

        
        
        for (int i = 0; i < numPlayers; i++)
        {
            GameObject playerControllerObject = new GameObject("PlayerController");
            LocalRacers.Insert(i, playerControllerObject.AddComponent<PlayerController>());
            _racers.Add(LocalRacers[i]);
        }
        
        
        // TODO: AI Racers
        int totalRacerCount = RaceManager.Instance.racerCount;
        int racerCount = --totalRacerCount;
        while (racerCount > 0)
        {
            GameObject aiControllerObject = new GameObject("AI Controller " + (totalRacerCount - racerCount));
            AIController aiController = aiControllerObject.AddComponent<AIController>();
            _racers.Add(aiController);
            racerCount--;
        }
        SetGameState(GameState.InMatch);
    }

    private void HandleGameStateSetInMatch()
    {
        UIManager.gameObject.SetActive(true);
        // checks to make sure we at least have enough normal players to instantiate. will be better later
        // gianluca
        if (numPlayers > RaceManager.Instance.racerCount)
        {
            RaceManager.Instance.racerCount = numPlayers;
        }
        RaceManager.Instance.StartRace();
    }

    private void HandleGameStateSetFinishedMatch()
    {
        SceneManager.LoadSceneAsync("Scenes/Main/StartMenu");
        SetGameState(GameState.MainMenu);
    }
    
    private void InitializeManagers()
    {
        GameObject inputManagerObject = Instantiate(inputManagerPrefab, gameObject.transform);
        GameObject UIManagerObject = Instantiate(UIManagerPrefab, gameObject.transform);
        GameObject audioManagerObject = Instantiate(AudioManagerPrefab, gameObject.transform);
        
        inputList = inputManagerObject.GetComponent<InputSys>();
        UIManager = UIManagerObject.GetComponent<RaceUI>();
        audioManager = audioManagerObject.GetComponent<AudioManager>();

        UIManager.gameObject.SetActive(false);
    }
    
    public List<RacerController> GetRacers()
    {
        return new List<RacerController>(_racers);
    }

    // TODO add weights to rarities
    public Item GetRandomItem()
    {
        return items[Random.Range(0,items.Count)];
    }

    // Singleplayer case
    public void SelectCharactersAndKarts(RacerSelection player1)
    {
        Player1 = player1;
    }

    // Multiplayer case
    public void SelectCharactersAndKarts(RacerSelection player1, RacerSelection player2)
    {
        Player1 = player1;
        Player2 = player2;
    }
}