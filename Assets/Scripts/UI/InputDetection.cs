using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputDetection : MonoBehaviour
{
    [SerializeField]
    private Button startButton;
    // only 1 keyboard can join
    private bool keyboardJoined = false;

    void Update()
    {
        if (Keyboard.current == null || GameManager.Instance.JoinedInputDevices.Count >= 4)
        {
            print("keyboard present or max players");
            return;
        }

        if (!keyboardJoined && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            GameManager.Instance.JoinedInputDevices.Enqueue(Keyboard.current);
            keyboardJoined = true;
            startButton.interactable = true;
            print("Keyboard joined");
        }

        foreach (Gamepad gamepad in Gamepad.all)
        {
            if (GameManager.Instance.JoinedInputDevices.Contains(gamepad))
            {
                continue;
            }
            if (gamepad.buttonSouth.wasPressedThisFrame)
            {
                GameManager.Instance.JoinedInputDevices.Enqueue(gamepad);
                startButton.interactable = true;
                print("Gamepad joined");
            }
        }
    }
}
