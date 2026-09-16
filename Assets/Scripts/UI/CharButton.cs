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
        Instantiate(targetCharacter.characterPrefab, modelLocation.transform);
        Instantiate(targetCharacter.defaultKart.kartPrefab, modelLocation.transform);
    }
    
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(PopulateInfo);
    }
}
