using UnityEngine;

public class Blight : Item
{
    [SerializeField]
    public GameObject glob;
    
    public override void Use(RacerState racer) {
        Drive drive = racer.RacerController.Drive;
        Vector3 playerPosition = drive.transform.position;
        BlightGlob globScript = glob.GetComponent<BlightGlob>();

        globScript.SetPlayerInitializer(racer);
        Instantiate(glob, playerPosition - drive.transform.forward * 4, Quaternion.identity);
    }
}
