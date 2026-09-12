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
    
    [SerializeField]
    private Kart selectedKart;
    
    public GameObject characterGameObject;
    
    public GameObject kartGameObject;
    
    private readonly int _spawnVerticalDisplacement = 2;

    public RacerState racerState {private set; get;}
    
    public ECharacterType GetCharacterType()
    {
        return characterType;
    }

    public Kart GetKart()
    {
        if (selectedKart != null) return selectedKart;
        
        return defaultKart;
    }

    private void TriggerAbility()
    {
        ability.TriggerAbility();
    }

    public void SetKart(Kart kart)
    {
        selectedKart = kart;
    }

    public void AssignRacerState(RacerState newRacerState)
    {
        racerState = newRacerState;
    }

    /// <summary>
    /// Provides the parent GameObject that holds both the racer and the car that are controlled by the player/AI
    /// </summary>
    /// <returns>parent RacerAndCar GameObject of the Character</returns>
    public GameObject GetRacerAndCar()
    {
        return characterGameObject.transform.parent.parent.gameObject;
    }

    public void Respawn()
    {
        Debug.Log("Respawning racer");
        GameObject racerAndCar = GetRacerAndCar();
        Rigidbody racerAndCarRigidbody = racerAndCar.GetComponent<Rigidbody>();
        racerAndCarRigidbody.linearVelocity = Vector3.zero;
        racerAndCarRigidbody.angularVelocity = Vector3.zero;
        TrackCheckpoint currentCheckpoint = racerState.currCheckpoint;
        racerAndCar.transform.position = currentCheckpoint.transform.position + Vector3.up * _spawnVerticalDisplacement;
        racerAndCar.transform.rotation = currentCheckpoint.GetTrackForwardDirection();
    }
}
