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
    float startsize = 5;
    float basefollowr = 5;

    CinemachineOrbitalFollow follow;
    SphereCollider sphereCollider;
    GameObject sphereObject;

    //jank ass shi...
    float timeRemaining = -2f;


    public override void Use()
    {
        //don't use while running
        if (timeRemaining > 0) return;
        timeRemaining = duration;

        //placeholder
        user = FindAnyObjectByType<PlayerController>();

        if (user is PlayerController) follow = ((PlayerController)user).CinemachineCamera.GetComponent<CinemachineOrbitalFollow>();

        //ok basically replaces the car with a sphere, really couldnt find a better/less jank solution
        GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Items/Snowball.prefab", typeof(GameObject)) as GameObject;
        //retain speed
        float currSpeed = user.Drive.m_currentSpeed;
        Destroy(user.Kart.kartObject);

        //create snowball in place of kart
        user.Kart.kartObject = Instantiate(prefab, user.Drive.transform.position + new Vector3(0f, 1f, 0f), user.Drive.transform.rotation);
        sphereCollider = user.Kart.kartObject.GetComponent<SphereCollider>();
        sphereObject = user.Kart.kartObject.transform.Find("Model").Find("Sphere").gameObject;
        
        //apply starting size
        sphereObject.transform.localScale = new Vector3(startsize, startsize, startsize);
        sphereCollider.radius = startsize / 2;

        //character still needs to be created for checkpoints and deathfloor
        GameObject characterSlot = user.Kart.kartObject.transform.Find("Model").Find("CharacterSocket").gameObject;
        user.Character.characterObject = Instantiate(user.Character.characterPrefab, Vector3.zero, user.Character.characterPrefab.transform.rotation);
        user.Character.characterObject.transform.SetParent(characterSlot.transform, false);

        //have to recreate drive ugh
        user.InitializeDrive();
        user.Drive.m_currentSpeed = currSpeed;

        //update follow
        if (user is PlayerController) {
            ((PlayerController)user).CinemachineCamera.Follow = user.Kart.kartObject.transform;
            basefollowr = follow.Radius;
        }
    }


    void Update()
    {
        if (timeRemaining > 0)
        {
            //primitive timer countdown lol
            timeRemaining -= Time.deltaTime;

            //sphere growth based on speed
            sphereObject.transform.localScale += new Vector3(user.Drive.m_currentSpeed * growthscale, user.Drive.m_currentSpeed * growthscale, user.Drive.m_currentSpeed * growthscale);
            sphereCollider.radius = sphereObject.transform.localScale.x / 2;
            
            //idea: insert rotate ball or increasing mass

            if (follow != null) follow.Radius = basefollowr + sphereObject.gameObject.transform.localScale.x / 2;
            
            //if too big, stop as well
            if (sphereObject.transform.localScale.x == maxsize) timeRemaining = 0;

        } else if (timeRemaining > -1)
        {
            //runs once so any "gets" are sorta fine imo

            //retains speed
            float currSpeed = user.Drive.m_currentSpeed;

            //kill ball
            Destroy(user.Kart.kartObject);

            //creates kart again
            user.Kart.kartObject = Instantiate(user.Kart.kartPrefab, user.Drive.transform.position + new Vector3(0f, 1f, 0f), user.Drive.transform.rotation);

            //and character
            GameObject characterSlot = user.Kart.kartObject.transform.Find("Model").Find("CharacterSocket").gameObject;
            user.Character.characterObject = Instantiate(user.Character.characterPrefab, Vector3.zero, user.Character.characterPrefab.transform.rotation);
            user.Character.characterObject.transform.SetParent(characterSlot.transform, false);

            //remake drive with retained speed
            user.InitializeDrive();
            user.Drive.m_currentSpeed = currSpeed;

            //reset camera
            if (user is PlayerController)
            {
                ((PlayerController)user).CinemachineCamera.Follow = user.Kart.kartObject.transform;
                follow.Radius = basefollowr;
            }

            //makes sure nothing runs after
            timeRemaining = -2;
        }

    }

    //event for hitting other drivers
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