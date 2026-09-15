using UnityEngine;

public class CharSelectScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Character[] characterArray = Resources.LoadAll("ScriptableObjects/Character", typeOf(ScriptableObject)) 
        as Character[];
         foreach (var c in characterArray)
        {
            Debug.Log(c.GetCharacterType());
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
