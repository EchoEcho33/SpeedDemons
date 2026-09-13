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
    
    public ECharacterType GetCharacterType()
    {
        return characterType;
    }

    private void TriggerAbility()
    {
        ability.TriggerAbility();
    }
}
