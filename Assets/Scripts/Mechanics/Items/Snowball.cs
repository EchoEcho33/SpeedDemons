using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Snowball : Item
{

    RacerController user;
    float duration = 10.0f;
    float maxsize = 100;
    float growthscale = 1 / 1500f;

    //jank ass shi...
    float timeRemaining = -2f;

    public override void Use()
    {
        //don't use while running
        if (timeRemaining > 0) return;
        timeRemaining = duration;

        //placeholder
        user = FindAnyObjectByType<PlayerController>();


        //ok basically replaces the car with a sphere, really couldnt find a better/less jank solution
        GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Items/Snowball.prefab", typeof(GameObject)) as GameObject;
        float currSpeed = user.Drive.m_currentSpeed;
        Destroy(user.Kart.kartObject);
       
        GameObject kartObject = Instantiate(prefab, user.Drive.transform.position + new Vector3(0f, 1f, 0f), user.Drive.transform.rotation);
        user.Kart.kartObject = kartObject;

        GameObject characterSlot = user.Kart.kartObject.transform.Find("Model").Find("CharacterSocket").gameObject; 
        GameObject characterObject = Instantiate(user.Character.characterPrefab, Vector3.zero, user.Character.characterPrefab.transform.rotation);
        characterObject.transform.SetParent(characterSlot.transform, false);
        user.Character.characterObject = characterObject;

        user.InitializeDrive();
        user.Drive.m_currentSpeed = currSpeed;

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
            user.Kart.kartObject.transform.Find("Model").Find("Sphere").gameObject.transform.localScale += new Vector3(user.Drive.m_currentSpeed * growthscale, user.Drive.m_currentSpeed * growthscale, user.Drive.m_currentSpeed * growthscale);
            user.Kart.kartObject.GetComponent<SphereCollider>().radius = user.Kart.kartObject.transform.Find("Model").Find("Sphere").gameObject.transform.localScale.x / 2;
            
            //idea of increasing mass as well
            //user.Kart.kartObject.GetComponent<Rigidbody>().mass = user.Kart.kartObject.transform.Find("Model").Find("Sphere").gameObject.transform.localScale.x / 10;

            //insert rotate ball if needed

            if (user is PlayerController)
            {
                ((PlayerController)user).CinemachineCamera.GetComponent<CinemachineOrbitalFollow>().Radius = 5 + user.Kart.kartObject.transform.Find("Model").Find("Sphere").gameObject.transform.localScale.x / 2;
            }

            //if too big, stop as well
            if (user.Kart.kartObject.transform.Find("Model").Find("Sphere").gameObject.transform.localScale.x == maxsize) timeRemaining = 0;

        } else if (timeRemaining > -1)
        {
            float currSpeed = user.Drive.m_currentSpeed;
            Destroy(user.Kart.kartObject);

            GameObject kartObject = Instantiate(user.Kart.kartPrefab, user.Drive.transform.position + new Vector3(0f, 1f, 0f), user.Drive.transform.rotation);
            user.Kart.kartObject = kartObject;

            GameObject characterSlot = user.Kart.kartObject.transform.Find("Model").Find("CharacterSocket").gameObject;
            GameObject characterObject = Instantiate(user.Character.characterPrefab, Vector3.zero, user.Character.characterPrefab.transform.rotation);
            characterObject.transform.SetParent(characterSlot.transform, false);
            user.Character.characterObject = characterObject;

            user.InitializeDrive();
            user.Drive.m_currentSpeed = currSpeed;
            if (user is PlayerController)
            {
                ((PlayerController)user).CinemachineCamera.Follow = user.Kart.kartObject.transform;
                ((PlayerController)user).CinemachineCamera.GetComponent<CinemachineOrbitalFollow>().Radius = 5;
            }

            timeRemaining = -2;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Drive drive = other.gameObject.GetComponentInParent<Drive>();
        if (drive == null) return;

        RacerController racer = drive.Racer;
        if (racer == null)
        {
            Debug.LogError("Racer not found.");
            return;
        }

        //placeholder for spinout
        racer.SpinOut = true;
        racer.SpinOutDuration = 3.0f;
    }
}