using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayerInput;

public class KeyboardRemap : MonoBehaviour
{
    public PlayerButton action;
    public Text actionName;
    bool remaping;
    public Text keyboardKey;

    private void Start()
    {
        actionName.text = action.ToString();
        keyboardKey.text = InputManager.playerButtons[action].keyboardKey.ToString();
    }

    public void Update()
    {
        if (remaping)
        {
            if (Input.anyKey)
            {
                foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(vKey))
                    {
                        SetButton(vKey);
                        remaping = false;
                    }
                }
            }
        }
    }
    public void Remaping()
    {
        remaping = true;
    }
    public void SetButton(KeyCode passed)
    {
        if (passed == KeyCode.W || passed == KeyCode.A || passed == KeyCode.S || passed == KeyCode.D) return;
        for (int i = 0; i < InputManager.playerButtons.Count; i++)
        {
            if (InputManager.playerButtons[(PlayerButton)i].keyboardKey == passed) return;
        }
        PlayerAction actn = InputManager.playerButtons[action];
        actn.keyboardKey = passed;
        InputManager.playerActions[(int)action] = actn;
        InputManager.playerButtons[action] = actn;
        keyboardKey.text = passed.ToString();
    }
}
