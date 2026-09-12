using System.Collections.Generic;
using Unity.Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Runtime.Serialization;

public class RaceManager : MonoBehaviour
{
    [Header("Level")]
    [SerializeField]
    private StartFinishCheckpoint startFinishCheckpoint;
    
    [SerializeField]
    private List<StartingGridSpot> startingGrid;

    [Header("Player")]
    [SerializeField]
    private Character selectedCharacter;
    
    [SerializeField]
    private Kart selectedKart;

    [SerializeField] 
    private GameObject cameraPrefab;
        
    [SerializeField] 
    private GameObject cinemachineCameraPrefab;

    [Header("Race Info")]
    [SerializeField]
    private List<Character> racers;
    private List<RacerState> _racerStates = new();

    //Do we need to create a separete class for ui elements?
    [SerializeField]
    public TMP_Text playerLaps;
    [SerializeField]
    public int maxLaps = 3;
    [SerializeField]
    public Image progressBar;

    //filler for example
    [SerializeField]
    public Image backgroundImage;


    // TODO: This is just a temporary count of how many racers should the manager spawn. The real count is the # of racers in _racerStates
    [SerializeField] 
    private int racerCount = 1;
    
    public void Start()
    {
        // TODO: Mike - I'm just doing this for now for testing purposes. Move this later!
        StartRace();
    }
    
    public void StartRace()
    {     
        if (selectedCharacter == null || selectedKart == null) return;

        // TODO: Mike - This only spawns 1 drive script for a singleplayer game. Change this for multiplayer!
        List<StartingGridSpot> remainingStartingGrid = startingGrid;
        int index = Random.Range(0, remainingStartingGrid.Count);
        StartingGridSpot startingGridSpot = startingGrid[index];
        remainingStartingGrid.RemoveAt(index);

        Character player = SpawnRacer(selectedCharacter, selectedKart, startingGridSpot);
        Drive drive = player.kartGameObject.AddComponent<Drive>();
        GameObject cameraGO = Instantiate(cameraPrefab);
        GameObject cinemachineCameraGO = Instantiate(cinemachineCameraPrefab);
        CinemachineCamera cinemachineCamera = cinemachineCameraGO.GetComponent<CinemachineCamera>();
        cinemachineCamera.Follow = player.kartGameObject.transform;
        drive.Initialize(player);
        
        racers.Remove(player);
        int count = --racerCount;
        
        foreach (Character racer in racers)
        {
            if (count <= 0) break;
            count--;
            
            index = Random.Range(0, remainingStartingGrid.Count);
            startingGridSpot = startingGrid[index];
            remainingStartingGrid.RemoveAt(index);
            
            SpawnRacer(racer, racer.GetKart(), startingGridSpot);
        }
    }

    private Character SpawnRacer(Character character, Kart kart, StartingGridSpot startingGridSpot)
    {
        // Initialize kart + character.
        GameObject kartObject = Instantiate(kart.kartPrefab, startingGridSpot.GetSpawnPoint(), kart.kartPrefab.transform.rotation);
        GameObject characterSlot = kartObject.transform.GetChild(0).gameObject; // First child should be the CharacterSlot!
        GameObject characterObject = Instantiate(character.characterPrefab, Vector3.zero, character.characterPrefab.transform.rotation); // TODO: Mike - The rotation of the character is currently just based on the prefab. 
        characterObject.transform.SetParent(characterSlot.transform, false);
        character.SetKart(kart);
        character.characterGameObject = characterObject;
        character.kartGameObject = kartObject;
        
        // Setup racer state.
        GameObject racerGO = new GameObject();
        RacerState racerState = racerGO.AddComponent<RacerState>();
        racerState.StartRace(startFinishCheckpoint);
        
        // Assign racer state to character.
        character.AssignRacerState(racerState);
        _racerStates.Add(racerState);
        
        return character;
    }
}