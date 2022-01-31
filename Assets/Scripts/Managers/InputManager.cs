using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerInput;
using System;
namespace PlayerInput {
    /// <summary>
    /// Enum for all Player axis actions (float vales)
    /// </summary>
    public enum PlayerAxis
    {
        MoveHorizontal,
        MoveVertical,
        CameraHorizontal,
        CameraVertical,
        LeftTrigger,
        RightTrigger,
        UI_Horizontal,
        UI_Vertical,
        None,
    };

    /// <summary>
    /// Enum for all Player button actions (bool values)
    /// </summary>
    [Serializable]
    public enum PlayerButton
    {
        Interact,
        Jump,
        Crouch,
        Freeze,
        Reverse,
        Slow,
        Global,
        Local,
        UI_Submit,     // UI Button
        UI_Cancel,     // UI Button
        Pause,
        None,
    };
    [Serializable]
    public struct PlayerAction 
    {
        public KeyCode keyboardKey;
        public KeyCode xboxKey;
    }
}
public class InputManager : MonoBehaviour {
    public enum InputMode {
        both,
        controller,
        keyboard
    }
    public static int inputType;
    public static InputMode inputMode = InputMode.both;
    // there is literally no reason for this to exist ffs
    [SerializeField]
    public static PlayerAction[] playerActions = new PlayerAction[12];

    public static Dictionary<KeyCode, string> playerXboxButtons = new Dictionary<KeyCode, string> {
        {KeyCode.JoystickButton0, "A"},
        {KeyCode.JoystickButton1, "B"},
        {KeyCode.JoystickButton2, "X"},
        {KeyCode.JoystickButton3, "Y"},
        {KeyCode.JoystickButton4,"Left Bumper"},
        {KeyCode.JoystickButton5, "Right Bumper"},
        {KeyCode.JoystickButton6, "Back"},
        {KeyCode.JoystickButton7, "Start"},
        {KeyCode.JoystickButton8, "L3"},
        {KeyCode.JoystickButton9, "R3"},
    };
    public static Dictionary<PlayerButton, PlayerAction> playerButtons = new Dictionary<PlayerButton, PlayerAction> { };
    public static Dictionary<PlayerAxis, string > joyAxis = new Dictionary <PlayerAxis, string> {
        {PlayerAxis.MoveHorizontal, "X Axis"},
        {PlayerAxis.MoveVertical, "Y Axis"},
        {PlayerAxis.CameraHorizontal, "4th Axis"},
        {PlayerAxis.CameraVertical, "5th Axis"},
        {PlayerAxis.LeftTrigger, "9th Axis"},
        {PlayerAxis.RightTrigger, "10th Axis"},
        {PlayerAxis.UI_Horizontal, "4th Axis"},
        {PlayerAxis.UI_Vertical, "5th Axis"},
    };

    public static Dictionary<PlayerAxis, string > mouseAxis = new Dictionary <PlayerAxis, string> {
        {PlayerAxis.MoveHorizontal, "KeyboardX"},
        {PlayerAxis.MoveVertical, "KeyboardY"},
        {PlayerAxis.CameraHorizontal, "MouseX"},
        {PlayerAxis.CameraVertical, "MouseY"},
        {PlayerAxis.UI_Horizontal, "KeyboardX"},
        {PlayerAxis.UI_Vertical, "KeyboardY"},
    };

    private void Start()
    {
        ResetKeycodes();
    }
    public static void ResetKeycodes () {
        playerActions[0].keyboardKey = KeyCode.E;
        playerActions[0].xboxKey = KeyCode.JoystickButton2;
        playerActions[1].keyboardKey = KeyCode.Space;
        playerActions[1].xboxKey = KeyCode.JoystickButton0;
        playerActions[2].keyboardKey = KeyCode.LeftControl;
        playerActions[2].xboxKey = KeyCode.JoystickButton1;
        playerActions[3].keyboardKey = KeyCode.F;
        playerActions[3].xboxKey = KeyCode.JoystickButton3;
        playerActions[4].keyboardKey = KeyCode.R;
        playerActions[4].xboxKey = KeyCode.JoystickButton4;
        playerActions[5].keyboardKey = KeyCode.Q;
        playerActions[5].xboxKey = KeyCode.JoystickButton5;
        playerActions[6].keyboardKey = KeyCode.Mouse0;
        playerActions[6].xboxKey = KeyCode.None;
        playerActions[7].keyboardKey = KeyCode.Mouse1;
        playerActions[7].xboxKey = KeyCode.None;
        playerActions[8].keyboardKey = KeyCode.Mouse0;
        playerActions[8].xboxKey = KeyCode.JoystickButton0;
        playerActions[9].keyboardKey = KeyCode.Escape;
        playerActions[9].xboxKey = KeyCode.JoystickButton1;
        playerActions[10].keyboardKey = KeyCode.Escape;
        playerActions[10].xboxKey = KeyCode.JoystickButton6;
        playerButtons[PlayerButton.Interact] = playerActions[0];
        playerButtons[PlayerButton.Jump] = playerActions[1];
        playerButtons[PlayerButton.Crouch] = playerActions[2];
        playerButtons[PlayerButton.Freeze] = playerActions[3];
        playerButtons[PlayerButton.Reverse] = playerActions[4];
        playerButtons[PlayerButton.Slow] = playerActions[5];
        playerButtons[PlayerButton.Global] = playerActions[6];
        playerButtons[PlayerButton.Local] = playerActions[7];
        playerButtons[PlayerButton.UI_Submit] = playerActions[8];
        playerButtons[PlayerButton.UI_Cancel] = playerActions[9];
        playerButtons[PlayerButton.Pause] = playerActions[10];
        if (inputType == 1) inputMode = InputMode.controller;
        if (inputType == 2) inputMode = InputMode.keyboard;
    }
    public static bool GetButtonDown(PlayerButton button) {
        bool input = false;
        input |= inputMode != InputMode.controller && Input.GetKeyDown(playerButtons[button].keyboardKey);
        input |= inputMode != InputMode.keyboard && Input.GetKeyDown(playerButtons[button].xboxKey);
        return input;
    }

    public static bool GetButtonUp (PlayerButton button) {
        bool input = false;
        input |= inputMode != InputMode.controller && Input.GetKeyUp(playerButtons[button].keyboardKey);
        input |= inputMode != InputMode.keyboard && Input.GetKeyUp(playerButtons[button].xboxKey);
        return input;
    }

    public static bool GetButton (PlayerButton button) {
        bool input = false;
        input |= inputMode != InputMode.controller && Input.GetKey(playerButtons[button].keyboardKey);
        input |= inputMode != InputMode.keyboard && Input.GetKey(playerButtons[button].xboxKey);
        return input;
    }
    public static float GetAxis (PlayerAxis axis) {
        var mouse = mouseAxis.ContainsKey(axis) ? Input.GetAxis(mouseAxis[axis]) : 0;
        var controller = joyAxis.ContainsKey(axis) ? Input.GetAxis(joyAxis[axis]) : 0;
        
        return (inputMode == InputMode.both && controller != 0) || inputMode == InputMode.controller ? controller : mouse;
    }
}