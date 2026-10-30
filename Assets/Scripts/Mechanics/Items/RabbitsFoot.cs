using UnityEngine;

public class RabbitsFoot : Item
{
    [SerializeField]
    private GameObject cloverVFX;

    public override void Use(RacerState racer)
    {
        racer.RacerController.activatedRF = true;
        racer.ApplyVFX(cloverVFX);
    }
}
