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
    public struct VoiceLine
    {
        [SerializeField]
        public string text;
        [SerializeField]
        public AudioClip audioFile;
    }

    [Serializable]
    public struct CharacterVoices
    {
        [SerializeField]
        public VoiceLine passing;
        [SerializeField]
        public VoiceLine gettingPassed;
        [SerializeField]
        public VoiceLine hitting;
        [SerializeField]
        public VoiceLine gettingHit;
        [SerializeField]
        public VoiceLine ult;
        [SerializeField]
        public VoiceLine winning;
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

    public CharacterVoices GetVoiceLines()
    {
        return VoiceLines;
    }
}
