using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSys : MonoBehaviour
{
    //TODO: delete this script? isnt really needed anymore
    [SerializeField] private InputActionAsset inputList;

    public InputActionAsset GetInputList()
    {
        return inputList;
    }
}