[System.Serializable]
public struct RacerSelection
{
    public Character character;
    
    public Kart kart;

    RacerSelection(Character character, Kart kart)
    {
        this.character = character;
        this.kart = kart;
    }
}
