using UnityEngine.InputSystem;

[System.Serializable]
public struct RacerSelection
{
    public Character character;
    
    public Kart kart;
    
    // the following vars are for when we update the character selection to support input selection
    
    // the device for this racer. will be null if not a player
    public InputDevice inputDevice;
    
    
    
    RacerSelection(Character character, Kart kart, InputDevice inputDevice)
    {
        this.character = character;
        this.kart = kart;
        this.inputDevice = inputDevice;
    }
    
    RacerSelection(Character character, Kart kart)
    {
        this.character = character;
        this.kart = kart;
        this.inputDevice = null;
    }
}
