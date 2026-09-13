using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class RaceManager : MonoBehaviour
{
    [Header("Level")]
    [SerializeField]
    private StartFinishCheckpoint startFinishCheckpoint;
    
    [SerializeField]
    private List<StartingGridSpot> startingGrid;

    [Header("Race Info")]
    [SerializeField]
    private List<RacerSelection> racerSelections;
    
    private List<RacerState> racerStates = new();

    [SerializeField]
    public TMP_Text playerLaps;
    [SerializeField]
    public int maxLaps = 3;
    
    // TODO: This is just a temporary count of how many racers should the manager spawn. The real count is the # of racers in _racerStates
    public int racerCount = 1;

    // FMOD script to allow us to set the player to be the 
    public FMODUnity.StudioListener studioListener;

    
    public void Start()
    {
        // TODO: Mike - I'm just doing this for now for testing purposes. Move this later!
        StartRace();
    }
    
    public void StartRace()
    {     
        if (racerSelections.Count == 0) return;

        // TODO: Mike - This only spawns 1 drive script for a singleplayer game. Change this for multiplayer!
        List<StartingGridSpot> remainingStartingGrid = startingGrid;
        int startingGridIndex = Random.Range(0, remainingStartingGrid.Count);
        StartingGridSpot startingGridSpot = startingGrid[startingGridIndex];
        remainingStartingGrid.RemoveAt(startingGridIndex);

        RacerSelection localRacerSelection = racerSelections[0]; // TODO: Currently, the local player selection is the first in the list
        (Character localCharacter, Kart localKart, RacerState localRacerState, GameObject playerObject) = SpawnRacer(localRacerSelection, startingGridSpot);
        PlayerController localRacer = GameManager.Instance.LocalRacer;
        localRacer.AssignCharacterAndKart(localCharacter, localKart);
        localRacer.AssignRacerState(localRacerState);

        // Sets the attenuarion object for FMOD to the players car. Will need to be updated for multiplayer probably
        studioListener = localRacer.MainCamera.GetComponent<FMODUnity.StudioListener>();
        studioListener.SetAttenuationObject(playerObject);
        // playerObject.AddComponent<TestCarHonk>();
        
        List<RacerController> racerControllers = GameManager.Instance.GetRacers();
        racerControllers.Remove(localRacer);
        int racerSelectionIndex = 0;
        
        foreach (RacerController racer in racerControllers)
        {
            startingGridIndex = Random.Range(0, remainingStartingGrid.Count);
            startingGridSpot = startingGrid[startingGridIndex];
            remainingStartingGrid.RemoveAt(startingGridIndex);
            
            (Character character, Kart kart, RacerState racerState, GameObject kartObject) = SpawnRacer(racerSelections[racerSelectionIndex], startingGridSpot);
            racer.AssignCharacterAndKart(character, kart);
            racer.AssignRacerState(racerState); 
            
            racerSelectionIndex++;
        }
    }

    private (Character, Kart, RacerState, GameObject) SpawnRacer(RacerSelection racerSelection, StartingGridSpot startingGridSpot)
    {
        Character character = Instantiate(racerSelection.character);
        Kart kart = Instantiate(racerSelection.kart);
        
        // Initialize kart + character.
        GameObject kartObject = Instantiate(kart.kartPrefab, startingGridSpot.GetSpawnPoint(), kart.kartPrefab.transform.rotation);
        GameObject characterSlot = kartObject.transform.GetChild(0).gameObject; // First child should be the CharacterSlot!
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
        
        return (character, kart, racerState, kartObject);
    }
}