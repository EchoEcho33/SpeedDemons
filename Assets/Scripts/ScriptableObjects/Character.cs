using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum ECharacterType
{
    None,
    Mothman,
    Nessie,
    Yeti,
    Jackalope,
    Nightcrawler
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

    public readonly int abilityCharge = 10;


    [Serializable]
    private struct VoiceLine
    {
        [SerializeField]
        private string text;
        [SerializeField]
        private AudioClip audioFile;
    }

    [Serializable]
    private struct CharacterVoices
    {
        [SerializeField]
        private VoiceLine passing;
        [SerializeField]
        private VoiceLine gettingPassed;
        [SerializeField]
        private VoiceLine hitting;
        [SerializeField]
        private VoiceLine gettingHit;
        [SerializeField]
        private VoiceLine ult;
        [SerializeField]
        private VoiceLine winning;
    }

    [SerializeField]
    private CharacterVoices VoiceLines;

    public ECharacterType GetCharacterType()
    {
        return characterType;
    }

    public void TriggerAbility(RacerState state)
    {
        ability.TriggerAbility(state);
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
