using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputDetection : MonoBehaviour
{
    [Header("Icons")]
    [SerializeField]
    private Sprite _keyboardIcon;
    [SerializeField]
    private Sprite _controllerIcon;

    [Header("UI Elements")]
    [SerializeField]
    private Button _startButton;

    [SerializeField]
    private List<Image> _inputIcons;

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
            _startButton.interactable = true;
            SetPlayerIcon(true, GameManager.Instance.JoinedInputDevices.Count);
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
                _startButton.interactable = true;
                SetPlayerIcon(false, GameManager.Instance.JoinedInputDevices.Count);
                print("Gamepad joined");
            }
        }
    }

    private void SetPlayerIcon(bool isKeyboard, int playerNum)
    {
        Sprite icon = isKeyboard ? _keyboardIcon : _controllerIcon;

        if (_inputIcons.Count < playerNum)
        {
            Debug.LogError("Not enough icons for amount of players joined.");
            return;
        }
            
        _inputIcons[playerNum-1].sprite = icon;
    }
}
