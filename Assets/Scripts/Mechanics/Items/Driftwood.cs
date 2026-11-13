using System.Collections;
using System.Data;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Driftwood : Item
{

    [Header("Driftwood Attributes")]

    [SerializeField] private float speed = 50;
    [SerializeField] private float distance = 150f;
    [SerializeField] private GameObject driftwoodProjectile;

    public override void Use(RacerState racer)
    {
        GameObject projectileObject = Instantiate(driftwoodProjectile, racer.RacerController.GetKartPosition(), racer.RacerController.GetCharacterAndKart().transform.rotation);
        DriftwoodProjectile projectile = projectileObject.GetComponent<DriftwoodProjectile>();
        projectile.SetForwardDirection(racer.RacerController.GetCharacterAndKart().transform.forward);
        projectile.SetRacer(racer);
        projectile.SetSpeed(speed);
        projectile.SetDistance(distance);
    }



}
