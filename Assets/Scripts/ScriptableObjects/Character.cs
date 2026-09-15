using Unity.VisualScripting;
using UnityEngine;

public enum ECharacterType
{
    None,
    Mothman,
    Nessie,
    Yeti,
    Jackalope
}

[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Character")]
public class Character : ScriptableObject
{
    [SerializeField]
    private ECharacterType characterType = ECharacterType.None;
    
    [SerializeField]
    private Kart defaultKart;

    [SerializeField]
    private Ability ability;

    [SerializeField]
    public GameObject characterPrefab;
    
    [HideInInspector]
    public GameObject characterObject;
    
    private readonly int _spawnVerticalDisplacement = 2;
    
    public ECharacterType GetCharacterType()
    {
        return characterType;
    }

    private void TriggerAbility()
    {
        ability.TriggerAbility();
    }

    /// <summary>
    /// Provides the parent GameObject that holds both the racer and the car that are controlled by the player/AI
    /// </summary>
    /// <returns>parent RacerAndCar GameObject of the Character</returns>
    public GameObject GetRacerAndCar()
    {
        return characterObject.transform.parent.parent.gameObject;
    }

    public void Respawn()
    {
        Debug.Log("Respawning racer");
        GameObject racerAndCar = GetRacerAndCar();
        Rigidbody racerAndCarRigidbody = racerAndCar.GetComponent<Rigidbody>();
        racerAndCarRigidbody.linearVelocity = Vector3.zero;
        racerAndCarRigidbody.angularVelocity = Vector3.zero;
        RacerController racerController = characterObject.GetComponentInParent<Drive>().Racer;
        TrackCheckpoint currentCheckpoint = racerController.RacerState.CurrCheckpoint;
        racerAndCar.transform.position = currentCheckpoint.transform.position + Vector3.up * _spawnVerticalDisplacement;
        racerAndCar.transform.rotation = currentCheckpoint.GetTrackForwardDirection();
    }
}
