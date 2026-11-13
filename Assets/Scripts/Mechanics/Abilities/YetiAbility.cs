using UnityEngine;

public class YetiAbility : Ability
{
    [SerializeField]
    private float _duration = 5.0f;
    [SerializeField]
    private float _speedLossMultiplier = 0.75f;

    public override void TriggerAbility(RacerState state)
    {
        foreach (RacerState racer in RaceManager.Instance.GetRacerStates())
        {
            if (!racer.Equals(state))
                racer.ApplyDebuff(RacerState.Debuff.SPEEDREDUCTION, _duration, _speedLossMultiplier);
        }
    }

}
