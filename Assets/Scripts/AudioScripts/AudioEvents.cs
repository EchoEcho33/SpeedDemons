using UnityEngine;
using FMODUnity;

public class AudioEvents : MonoBehaviour
{
    [field: Header("Sean honk")]
    [field: SerializeField] public EventReference seanHonk {get; private set;}

    [field: Header("Car SFX")]
    [field: SerializeField] public EventReference basicCarDrive {get; private set;}

    [field: Header("Car Impact SFX")]
    [field: SerializeField] public EventReference hitItemBoxPlayer {get; private set;}
    [field: SerializeField] public EventReference hitItemBoxAI {get; private set;}
    public static AudioEvents instance {get; private set;}
    [field: SerializeField] public EventReference useItem {get; private set;}

    [field: Header("Music")]
    [field: SerializeField] public EventReference characterSelectMusic {get; private set;}
    [field: SerializeField] public EventReference mainGameMusic {get; private set;}

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Someone pulled a big stupid and added a second AudioEvents");
        }
        instance = this;
    }
}
