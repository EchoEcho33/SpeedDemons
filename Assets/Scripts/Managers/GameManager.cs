using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputSys), typeof(RaceManager))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector]
    public InputSys input;
    
    [HideInInspector]
    public RaceManager race;
    
    [SerializeField] 
    public GameObject cameraPrefab;
        
    [SerializeField] 
    public GameObject cinemachineCameraPrefab;
    
    public PlayerController LocalRacer { get; private set; }
    
    private List<RacerController> racers = new();

    [HideInInspector]
    public RespawnManager respawnManager;
    
    [Header("Databases")]
    [SerializeField]
    public List<Item> items;

    [SerializeField]
    public List<Character> characters;

    public void Awake()
    {
        if (Instance == null) Instance = this;

        input = FindFirstObjectByType<InputSys>();
        race = FindFirstObjectByType<RaceManager>();
        respawnManager = FindFirstObjectByType<RespawnManager>();

        // TODO: The Local Player driven by Inputs, maybe multiplayer?
        GameObject playerControllerObject = new GameObject("PlayerController");
        LocalRacer = playerControllerObject.AddComponent<PlayerController>();
        racers.Add(LocalRacer);

        // TODO: AI Racers
        int totalRacerCount = race.racerCount;
        int racerCount = --totalRacerCount;
        while (racerCount > 0)
        {
            GameObject aiControllerObject = new GameObject("AI Controller " + (totalRacerCount - racerCount));
            AIController aiController = aiControllerObject.AddComponent<AIController>();
            racers.Add(aiController);
            racerCount--;
        }
    }

    public List<RacerController> GetRacers()
    {
        return new List<RacerController>(racers);
    }
}