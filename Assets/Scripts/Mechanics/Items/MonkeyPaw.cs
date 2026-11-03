using System.Collections;
using System.Data;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class MonkeyPaw : Item
{

    [Header("Monkey's Paw Attributes")]
    public float boost = 1.2f;
    public float decel = 0.8f;
    public float duration_up = 2;
    public float duration_down = 2;

    public override void Use(RacerState racer)
    {
        racer.ApplyBuff(RacerState.Buff.SPEEDBOOST, duration_up, boost);
        StartCoroutine(DebuffTimer(duration_up, racer));
    }

    private IEnumerator DebuffTimer(float timer, RacerState racer)
    {
        yield return new WaitForSeconds(timer);
        racer.ApplyDebuff(RacerState.Debuff.SPEEDREDUCTION, duration_down, decel);
    }

}
