using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Character")]
public class Character : ScriptableObject
{
    [SerializeField]
    private Kart defaultKart;
    [SerializeField]
    private Ability ability;

    private Kart selectedKart;

    public RacerState racerState {private set; get;}
    

    public Kart GetKart()
    {
        return defaultKart;
    }

    private void TriggerAbility()
    {
        ability.TriggerAbility();
    }

    public void AssignRacerState(RacerState newRacerState)
    {
        racerState = newRacerState;
    }
}
