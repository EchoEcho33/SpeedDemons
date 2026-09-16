using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class CharButton : Button
{
    
    private Character targetCharacter;
    private TextMeshProUGUI abilityNameBox, abilityDescBox, kartStatsBox;
    private Image abilityIconBox;
    private GameObject modelLocation;
    public void GetInfo(Character c, TextMeshProUGUI ability, TextMeshProUGUI abilityDesc,  Image abilityIcon, TextMeshProUGUI kartStats, GameObject mLocation)
    {
        targetCharacter = c;
        this.image.sprite = c.PolaroidIcon;
        
        abilityNameBox = ability;
        abilityDescBox = abilityDesc;
        abilityIconBox = abilityIcon;
        kartStatsBox = kartStats;
        modelLocation = mLocation;
        
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
        kartStatsBox.text = targetCharacter.defaultKart.name;
        foreach (Transform child in modelLocation.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        
        SpawnRacerModel(targetCharacter, targetCharacter.defaultKart, modelLocation);
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
