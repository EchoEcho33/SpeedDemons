using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class CharSelect : MonoBehaviour
{
    [SerializeField]
    private Character[] characters;
    [SerializeField]
    private Text AbilityText;
    [SerializeField]
    private Text KartText;

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
        foreach (Character c in characters) 
        {
            Debug.Log(c);
        }


    }   
}
