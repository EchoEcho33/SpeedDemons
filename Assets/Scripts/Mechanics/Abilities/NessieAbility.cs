using UnityEngine;

public class NessieAbility : Ability
{
    [SerializeField]
    public GameObject puddle; // For the Puddle Prefab

    public override void TriggerAbility(RacerState state) {
        Drive drive = state.RacerController.Drive;
        Vector3 playerPosition = state.RacerController.Drive.transform.position;
        HydraPlanePuddle puddleScript = puddle.GetComponent<HydraPlanePuddle>();

        if (puddle == null)
        {
            Debug.LogError("HydraPlanePuddle does not contain HydraPlanePuddle script.");
            return;
        }

        puddleScript.SetPlayerInitializer(state);
        Instantiate(puddle, playerPosition - drive.transform.forward * 4, Quaternion.identity);
    }
}
