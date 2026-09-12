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
}