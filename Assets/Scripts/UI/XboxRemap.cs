using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayerInput;
using System.Linq;

public class XboxRemap : MonoBehaviour
{
    public PlayerButton action;
    public Text xboxKey;

    private void Start()
    {
        xboxKey.text = InputManager.playerXboxButtons[InputManager.playerButtons[action].xboxKey];
    }
    bool remaping;

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
        StartCoroutine(timerRemaping());
    }
    public void SetButton(KeyCode passed)
    {
        for (int i = 0; i < InputManager.playerButtons.Count; i++)
        {
            if (InputManager.playerButtons[(PlayerButton)i].xboxKey == passed) return;
        }
        if (InputManager.playerXboxButtons.ContainsKey(passed))
        {
            PlayerAction actn = InputManager.playerButtons[action];
            actn.xboxKey = passed;
            InputManager.playerActions[(int)action] = actn;
            InputManager.playerButtons[action] = actn;
            xboxKey.text = passed.ToString();
        }
    }
    public IEnumerator timerRemaping()
    {
        yield return new WaitForSeconds(.5F);
        remaping = true;
    }
}

