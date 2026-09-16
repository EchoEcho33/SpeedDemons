using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class CharSelect : MonoBehaviour
{
    [SerializeField]
    private List<Character> characters;

    [SerializeField] 
    private GameObject buttonContainer;

    [SerializeField] private TextMeshProUGUI abilityName, abilityDesc, kartText; 
    [SerializeField]
    private Image abilityIcon;
    [SerializeField]
    private GameObject modelLocation;
    

// will be replaced with progress bars
    [Header("Kart Stats Text")]
    [SerializeField]
    private Text maxSpeed;
    [SerializeField]
    private Text maxAccel;
    [SerializeField]
    private Text brakeStrength;
    [SerializeField]
    private Text drag;
    [SerializeField]
    private Text turnRadius;
    [SerializeField]
    private Text traction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<CharButton> buttons = new List<CharButton>();
          
        foreach (Transform child in buttonContainer.transform)
        {
            buttons.Add(child.GetComponent<CharButton>());
        }

        if (characters.Count > buttons.Count)
        {
            throw new IndexOutOfRangeException("More characters than buttons! Add more CharacterButton prefabs to the UI under 'Polaroids' "); 
        }
        
        for (int i = 0; i < characters.Count; i++)
        {
            buttons[i].GetInfo(characters[i], abilityName, abilityDesc, abilityIcon, kartText, modelLocation);
            Debug.Log(buttons[i] + " has character " + characters[i].name);
        }

    }   
}
