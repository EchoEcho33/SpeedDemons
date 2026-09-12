using UnityEngine;

public abstract class RacerController : MonoBehaviour
{
    public Character Character { get; protected set; }
    
    public Kart Kart { get; protected set; }
    
    public Drive Drive { get; protected set; }
    
    public RacerState RacerState { get; private set; }
    
    public void AssignRacerState(RacerState newRacerState)
    {
        RacerState = newRacerState;
        RacerState.AssignController(this);
    }

    public virtual void AssignCharacterAndKart(Character newCharacter, Kart newKart)
    {
        Character = newCharacter;
        Kart = newKart;
        
        InitializeDrive();
    }

    protected abstract void InitializeDrive();
    
    /// <summary>
    /// Provides the parent GameObject that holds both the racer character and the car that are controlled by the player/AI
    /// </summary>
    /// <returns>parent RacerAndCar GameObject of the Character</returns>
    public GameObject GetCharacterAndKart()
    {
        return Character.characterObject.transform.parent.parent.gameObject;
    }
}