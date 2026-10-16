using UnityEngine;

public class RabbitsFoot : Item
{
    public override void Use(RacerState racer)
    {
        racer.RacerController.activatedRF = true;
    }
}
