using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum GameState
{
    MainMenu,
    Lobby,
    WaitingToStart,
    InMatch
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set;}

    public InputSys inputManager { get; private set;}

    public UIManager UIManager { get; private set;}
    
    public AudioManager audioManager { get; private set;}
    
#if UNITY_EDITOR
    public RacerRecorder racerRecorder { get; private set;}
#endif
    
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
    public GameState startingGameState = GameState.WaitingToStart;

    public GameState GameState { get; private set; }
    
    public PlayerController LocalRacer { get; private set; }
    
    private List<RacerController> _racers = new();
    public List<RacerController> Racers => new(_racers);
    
    [Header("Databases")]
    [SerializeField]
    public List<Item> items;

    [SerializeField]
    public List<Character> characters;

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
        
        // TODO: This currently how all the players are initialized. This should be instead be populated via fields in a lobby.
        // TODO: The Local Player driven by Inputs, maybe multiplayer?
        GameObject playerControllerObject = new GameObject("PlayerController");
        LocalRacer = playerControllerObject.AddComponent<PlayerController>();
        _racers.Add(LocalRacer);

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

        // TODO: This is just to start the race have the match is setup.
        SetGameState(GameState.InMatch);
    }

    private void HandleGameStateSetInMatch()
    {
        RaceManager.Instance.StartRace();
    }
    
    private void InitializeManagers()
    {
        GameObject inputManagerObject = Instantiate(inputManagerPrefab, gameObject.transform);
        GameObject UIManagerObject = Instantiate(UIManagerPrefab, gameObject.transform);
        GameObject audioManagerObject = Instantiate(AudioManagerPrefab, gameObject.transform);
        
        inputManager = inputManagerObject.GetComponent<InputSys>();
        UIManager = UIManagerObject.GetComponent<UIManager>();
        audioManager = audioManagerObject.GetComponent<AudioManager>();
    }
    
    public List<RacerController> GetRacers()
    {
        return new List<RacerController>(_racers);
    }
}