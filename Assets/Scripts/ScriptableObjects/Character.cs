using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    public Ability ability;

    [SerializeField]
    public GameObject characterPrefab;

    [SerializeField]
    public Sprite PolaroidIcon;
    
    [HideInInspector]
    public GameObject characterObject;
    
    public readonly int _spawnVerticalDisplacement = 2;

    private GameObject snowball;

    public ECharacterType GetCharacterType()
    {
        return characterType;
    }

    public void TriggerAbility()
    {
        ability.TriggerAbility();
    }
    
    // Some getters necessary for the character selection UI.
    public Kart getDefaultKart()
    {
        return defaultKart;
    }
    
    public Ability getAbility()
    {
        return ability;
    }
}
