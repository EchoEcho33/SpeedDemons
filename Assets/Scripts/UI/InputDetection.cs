using UnityEngine;
using UnityEngine.InputSystem;

public class InputDetection : MonoBehaviour
{
    // only 1 keyboard can join
    private bool keyboardJoined = false; 

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (!keyboardJoined && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            GameManager.Instance.JoinedInputDevices.Enqueue(Keyboard.current);
            keyboardJoined = true;
            print("Keyboard joined");
        }

        foreach (var gamepad in Gamepad.all)
        {
            if (GameManager.Instance.JoinedInputDevices.Contains(gamepad))
            {
                continue;
            }
            if (gamepad.buttonSouth.wasPressedThisFrame)
            {
                GameManager.Instance.JoinedInputDevices.Enqueue(gamepad);
                print("Gamepad joined");
            }
        }
    }
}
