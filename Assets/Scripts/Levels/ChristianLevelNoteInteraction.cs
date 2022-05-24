using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChristianLevelNoteInteraction : InteractiveNote
{
    public Flowing_River fr;
    public string customMessageOne = "";
    public string customMessageTwo = "";
    public string customMessageThree = "";
    public string customMessageFour = "";

    public PlayerInput.PlayerButton slowButton;
    private PlayerInput.PlayerAction slowAction;
    public PlayerInput.PlayerButton objectButton;
    private PlayerInput.PlayerAction objectAction;
    public PlayerInput.PlayerButton worldButton;
    private PlayerInput.PlayerAction worldAction;
    public override void Interact()
    {
        PauseMenu.singleton.DisplayNote(noteContent);
        fr.streamForce = 1;

        slowAction = InputManager.playerButtons[slowButton];
        objectAction = InputManager.playerButtons[objectButton];
        worldAction = InputManager.playerButtons[worldButton];

        string messageS;
        string messageO;
        string messageW;
        if (InputManager.inputMode == InputManager.InputMode.keyboard)
        {
            messageS = slowAction.keyboardKey.ToString();
            messageO = objectAction.keyboardKey.ToString();
            messageW = worldAction.keyboardKey.ToString();
        }
        else
        {
            messageS = InputManager.playerXboxButtons[slowAction.xboxKey];
            messageO = InputManager.joyAxis[PlayerInput.PlayerAxis.RightTrigger];
            messageW = InputManager.joyAxis[PlayerInput.PlayerAxis.LeftTrigger];
        }

        string message = customMessageOne + messageS + customMessageTwo + messageO + customMessageThree + messageW + customMessageFour;
        PauseMenu.singleton.PrepCustomMessage(message);
    }
}
