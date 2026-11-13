using UnityEngine;

public class NefariousBrew : Item
{
    [SerializeField]
    private int abilityBarAddition;

    public override void Use(RacerState racer) {
        racer.UpdateBar(abilityBarAddition);
    }
}
