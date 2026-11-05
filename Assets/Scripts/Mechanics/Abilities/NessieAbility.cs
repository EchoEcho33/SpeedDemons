using UnityEngine;

public class NessieAbility : Ability
{
    [SerializeField]
    public GameObject puddle; // For the Puddle Prefab

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override int _triggerCost() {
        return 0;
    }

    public override void TriggerAbility(RacerState state) {
        Drive drive = state.RacerController.Drive;
        Vector3 playerPosition = state.RacerController.Drive.transform.position;
        Instantiate(puddle, playerPosition - drive.transform.forward * 4, Quaternion.identity);
    }
}
