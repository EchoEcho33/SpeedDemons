using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Diagnostics;

public class AudioManager : MonoBehaviour
{
    private List<EventInstance> instanceList;
    private List<StudioEventEmitter> emitterList;
    public static AudioManager instance {get; private set;}

    private void Awake()
    {
        if(instance != null)
        {
            UnityEngine.Debug.LogError("Someone pulled a big stupid and added a second AudioManager");
        }
        instance = this;

        instanceList = new List<EventInstance>();
        emitterList = new List<StudioEventEmitter>();
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance createInstance(EventReference eventReference)
    {
        EventInstance toReturn = RuntimeManager.CreateInstance(eventReference);
        instanceList.Add(toReturn);
        return toReturn;
    }

    public StudioEventEmitter createEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        UnityEngine.Debug.Log("adding emitter");
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        UnityEngine.Debug.Log(emitter);
        emitter.EventReference = eventReference;
        emitterList.Add(emitter);
        return emitter;
    }

    public void cleanUp()
    {
        foreach(EventInstance eventInstance in instanceList)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }

        foreach(StudioEventEmitter emitter in emitterList)
        {
            emitter.Stop();
        }
    }

    public void OnDestroy()
    {
        cleanUp();
    }
}
