using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class CharacterSelectAudio : MonoBehaviour
{
    public StudioEventEmitter backgroundMusic;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // idk why this is crashing im commenting it out for now - gianluca
        //backgroundMusic = AudioManager.instance.createEmitter(AudioEvents.instance.characterSelectMusic, this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
