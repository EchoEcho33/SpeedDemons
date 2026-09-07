using UnityEngine;
using FMODUnity;

public class AudioEvents : MonoBehaviour
{
    [field: Header("Sean honk")]
    [field: SerializeField] public EventReference seanHonk {get; private set;}
    public static AudioEvents instance {get; private set;}

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Someone pulled a big stupid and added a second AudioEvents");
        }
        instance = this;
    }
}
