using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemstoneUnlockMessage : InteractiveNote
{
    public string customMessageOne = "";
    public string customMessageTwo = "";
    public string customMessageThree = "";
    public string customMessageFour = "";

    public PlayerInput.PlayerButton freezeButton;
    private PlayerInput.PlayerAction freezeAction;
    public PlayerInput.PlayerButton objectButton;
    private PlayerInput.PlayerAction objectAction;
    public PlayerInput.PlayerButton worldButton;
    private PlayerInput.PlayerAction worldAction;
    public override void Interact()
    {
        PauseMenu.singleton.DisplayNote(noteContent);

        freezeAction = InputManager.playerButtons[freezeButton];
        objectAction = InputManager.playerButtons[objectButton];
        worldAction = InputManager.playerButtons[worldButton];

        string messageS;
        string messageO;
        string messageW;
        if (InputManager.inputMode == InputManager.InputMode.keyboard)
        {
            messageS = freezeAction.keyboardKey.ToString();
            messageO = objectAction.keyboardKey.ToString();
            messageW = worldAction.keyboardKey.ToString();
        }
        else
        {
            messageS = InputManager.playerXboxButtons[freezeAction.xboxKey];
            messageO = InputManager.playerXboxButtons[objectAction.xboxKey];
            messageW = InputManager.playerXboxButtons[worldAction.xboxKey];
        }

        string message = customMessageOne + messageS + customMessageTwo + messageO + customMessageThree + messageW + customMessageFour;
        PauseMenu.singleton.PrepCustomMessage(message);

        GetComponent<AbilityUnlock>().UnlockAbility();
    }
}
