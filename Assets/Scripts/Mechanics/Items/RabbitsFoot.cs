using UnityEngine;

public class RabbitsFoot : Item
{
    [SerializeField]
    private GameObject cloverVFX;

    public override void Use(RacerState racer)
    {
        racer.ApplyBuff(RacerState.Buff.BARRIER);
        racer.ApplyVFX(cloverVFX);
    }
}
