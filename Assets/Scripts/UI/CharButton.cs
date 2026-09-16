using System;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class CharButton : Button
{

    private CharSelect UIController; 
    private Character targetCharacter;
    private TextMeshProUGUI abilityNameBox, abilityDescBox, kartStatsBox;
    private Image abilityIconBox, speedRating, accelRating, brakeRating, dragRating;
    private GameObject modelLocation;
    public void GetInfo
        (CharSelect ui, Character c, TextMeshProUGUI ability, TextMeshProUGUI abilityDesc,  Image abilityIcon, TextMeshProUGUI kartStats, GameObject mLocation,
            Image speed, Image accel, Image brake, Image drag)
    {
        UIController = ui;
        targetCharacter = c;
        this.image.sprite = c.PolaroidIcon;
        
        abilityNameBox = ability;
        abilityDescBox = abilityDesc;
        abilityIconBox = abilityIcon;
        kartStatsBox = kartStats;
        modelLocation = mLocation;
        
        speedRating = speed;
        accelRating = accel;
        brakeRating = brake;
        dragRating = drag;
    }

    public void PopulateInfo()
    {
        Debug.Log("button: " + gameObject.name);
        Debug.Log("targetCharacter: " + targetCharacter);
        Debug.Log("abilityNameBox: " + abilityNameBox);
        Debug.Log("abilityDescBox: " + abilityDescBox);
        if (targetCharacter.ability != null)
        {
            abilityNameBox.text = targetCharacter.ability.abilityName;
            abilityDescBox.text = targetCharacter.ability.description;
            abilityIconBox.sprite = targetCharacter.ability.icon;
        }
        else
        {
            abilityNameBox.text = "No ability";
            abilityDescBox.text = "No ability description";
            
        }
        // kart stats
        Kart kart = targetCharacter.defaultKart;
        kartStatsBox.text = kart.name;
        speedRating.fillAmount = Mathf.Clamp01(kart._maxSpeed / 50f);
        accelRating.fillAmount = Mathf.Clamp01(kart._maxAcceleration / 20f);
        brakeRating.fillAmount = Mathf.Clamp01(kart._brakeStrength / 15f);
        dragRating.fillAmount = Mathf.Clamp01(kart._drag / 5f);
        
        foreach (Transform child in modelLocation.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        
        SpawnRacerModel(targetCharacter, targetCharacter.defaultKart, modelLocation);

        RacerSelection racer = new RacerSelection();
        racer.character = targetCharacter;
        racer.kart = targetCharacter.defaultKart;
        
        UIController.SaveSelection(racer);
    }
    
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(PopulateInfo);
    }
    
    private void SpawnRacerModel(Character character, Kart kart, GameObject spawnLocation)
    {
        // Initialize kart + character.
        GameObject kartObject = Instantiate(kart.kartPrefab, spawnLocation.transform);
        GameObject characterSlot = kartObject.transform.GetChild(0).gameObject; // First child should be the CharacterSlot!
        GameObject characterObject = Instantiate(character.characterPrefab, Vector3.zero, character.characterPrefab.transform.rotation); // TODO: Mike - The rotation of the character is currently just based on the prefab. 
        characterObject.transform.SetParent(characterSlot.transform, false);
        character.characterObject = characterObject;
        kart.kartObject = kartObject;
    }
}
