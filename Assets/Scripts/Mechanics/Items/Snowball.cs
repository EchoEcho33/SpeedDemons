using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Snowball : Item
{

    RacerController user;
    float duration = 10.0f;
    float timeRemaining = -2f;

    public override void Use()
    {

        if (timeRemaining > 0) return;
        timeRemaining = duration;

        //placeholder
        user = FindAnyObjectByType<PlayerController>();

        GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Items/Snowball.prefab", typeof(GameObject)) as GameObject;
        Destroy(user.Kart.kartObject);
       
        GameObject kartObject = Instantiate(prefab, user.Drive.transform.position + new Vector3(0f, 1f, 0f), prefab.transform.rotation);
        GameObject characterSlot = user.Kart.kartObject.transform.Find("Model").Find("CharacterSocket").gameObject; 
        GameObject characterObject = Instantiate(user.Character.characterPrefab, Vector3.zero, user.Character.characterPrefab.transform.rotation);
        if (characterObject == null) { Debug.Log("Kill Yourself"); }
        characterObject.transform.SetParent(characterSlot.transform, false);
        user.Character.characterObject = characterObject;
        user.Kart.kartObject = kartObject;

        user.InitializeDrive();
        if (user is PlayerController)
        {
            ((PlayerController)user).CinemachineCamera.Follow = user.Kart.kartObject.transform;
        }

    }


    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            user.Kart.kartObject.transform.localScale += new Vector3(user.Drive.m_currentSpeed/1000f, user.Drive.m_currentSpeed / 1000f, user.Drive.m_currentSpeed / 1000f);

        } else if (timeRemaining > -1)
        {
            Destroy(user.Kart.kartObject);

            GameObject kartObject = Instantiate(user.Kart.kartPrefab, user.Drive.transform.position + new Vector3(0f, 1f, 0f), user.Kart.kartPrefab.transform.rotation);
            GameObject characterSlot = user.Kart.kartObject.transform.Find("Model").Find("CharacterSocket").gameObject;
            GameObject characterObject = Instantiate(user.Character.characterPrefab, Vector3.zero, user.Character.characterPrefab.transform.rotation);
            characterObject.transform.SetParent(characterSlot.transform, false);
            user.Character.characterObject = characterObject;
            user.Kart.kartObject = kartObject;

            user.InitializeDrive();
            if (user is PlayerController)
            {
                ((PlayerController)user).CinemachineCamera.Follow = user.Kart.kartObject.transform;
            }

            timeRemaining = -2;
        }

    }
}