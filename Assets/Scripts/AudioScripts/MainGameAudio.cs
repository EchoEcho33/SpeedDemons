using FMODUnity;
using UnityEngine;

public class MainGameAudio : MonoBehaviour
{
    public static MainGameAudio instance;

    // MAIN BACKGROUND MUSIC
    public StudioEventEmitter backgroundMusic;

    // LISTS FOR VOICE CLIPS


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Someone pulled a big stupid and added a second AudioEvents");
        }
        instance = this;
    }
    void Start()
    {
        backgroundMusic = AudioManager.instance.createEmitter(AudioEvents.instance.mainGameMusic, this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
