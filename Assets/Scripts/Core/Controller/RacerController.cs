using UnityEngine;

public abstract class RacerController : MonoBehaviour
{
    public Character Character { get; protected set; }
    
    public Kart Kart { get; protected set; }
    
    public Drive Drive { get; protected set; }
    
    public RacerState RacerState { get; private set; }

    public bool SpinOut = false;
    public float SpinOutDuration = 0;
    
    public float Steering { get; protected set; }
    
    public float Throttle { get; protected set; }
    
    public float Brake { get; protected set; }

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
        return Character.characterObject.transform.root.gameObject;
    }

    public Vector3 GetKartPosition()
    {
        return GetCharacterAndKart().transform.position;
    }
}