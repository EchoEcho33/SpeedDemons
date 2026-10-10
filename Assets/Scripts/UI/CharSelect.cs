using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using Unity.VectorGraphics;

public class CharSelect : MonoBehaviour
{
    public RacerSelection racerSelection;

    [SerializeField] 
    private GameObject buttonContainer;

    [SerializeField] private TextMeshProUGUI abilityName, abilityDesc, kartText; 
    [SerializeField]
    private Image abilityIcon;
    [SerializeField]
    private GameObject modelLocation;

    [Header("Kart Stats Text")]
    [SerializeField]
    private Image speedRating;
    [SerializeField]
    private Image accelRating;
    [SerializeField]
    private Image brakeRating;
    [SerializeField]
    private Image dragRating;

    private bool _singlePlayer = true;
    void Start()
    {
        List<CharButton> buttons = new List<CharButton>();
          
        foreach (Transform child in buttonContainer.transform)
        {
            buttons.Add(child.GetComponent<CharButton>());
        }

        if (GameManager.Instance.characters.Count > buttons.Count)
        {
            throw new IndexOutOfRangeException("More characters than buttons! Add more CharacterButton prefabs to the UI under 'Polaroids' "); 
        }
        
        for (int i = 0; i < GameManager.Instance.characters.Count; i++)
        {
            buttons[i].GetInfo
                (this, GameManager.Instance.characters[i], abilityName, abilityDesc, abilityIcon, kartText, modelLocation, speedRating, accelRating, brakeRating, dragRating);
            Debug.Log(buttons[i] + " has character " + GameManager.Instance.characters[i].name);
        }

    }

    public void SaveSelection(RacerSelection racer)
    {
        racerSelection = racer; 
    }

    public bool IsSingleplayer()
    {
        return _singlePlayer;
    }
}
