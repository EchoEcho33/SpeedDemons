using System;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(InputSys), typeof(RaceManager))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector]
    public InputSys input;
    
    [HideInInspector]
    public RaceManager race;

    public void Awake()
    {
        input = FindFirstObjectByType<InputSys>();
        race = FindFirstObjectByType<RaceManager>();

        if (Instance == null)
            Instance = this;
    }
}