using UnityEngine;

public class NessieAbility : Ability
{
    [SerializeField]
    public GameObject puddle; // For the Puddle Prefab

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override int _triggerCost() {
        return 0;
    }

    public override void TriggerAbility() {
        // This Spawn Code is Untested, If it breaks, whoopsies -Kenneth
        Vector3 playerPosition = this.gameObject.transform.root.position;
        Instantiate(puddle, new Vector3(0, 0, -2), Quaternion.identity);
    }
}
