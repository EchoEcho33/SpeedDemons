using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    
    private List<RacerController> _racers = new();
    public List<RacerController> Racers => new(_racers);
    
    [Header("Databases")]
    [SerializeField]
    public List<Item> items;

    [SerializeField]
    public List<Character> characters;

    [Header("UI")]
    [SerializeField]
    private Canvas _mainRaceCanvas;
    [SerializeField]
    private Image _primaryItemIcon;
    [SerializeField]
    private Image _secondaryItemIcon;

    public void Awake()
    {
        if (Instance == null) Instance = this;

        input = FindFirstObjectByType<InputSys>();
        race = FindFirstObjectByType<RaceManager>();

        // TODO: The Local Player driven by Inputs, maybe multiplayer?
        GameObject playerControllerObject = new GameObject("PlayerController");
        LocalRacer = playerControllerObject.AddComponent<PlayerController>();
        _racers.Add(LocalRacer);

        // TODO: AI Racers
        int totalRacerCount = race.racerCount;
        int racerCount = --totalRacerCount;
        while (racerCount > 0)
        {
            GameObject aiControllerObject = new GameObject("AI Controller " + (totalRacerCount - racerCount));
            AIController aiController = aiControllerObject.AddComponent<AIController>();
            _racers.Add(aiController);
            racerCount--;
        }
    }

    public List<RacerController> GetRacers()
    {
        return new List<RacerController>(_racers);
    }
}