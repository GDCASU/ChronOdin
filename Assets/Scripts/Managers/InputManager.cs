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
        Sprint,
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
    public static InputMode inputMode;
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
        {PlayerAxis.MoveHorizontal, "Left Joystick Horizontal"},
        {PlayerAxis.MoveVertical, "Left Joystick Vertical"},
        {PlayerAxis.CameraHorizontal, "Right Joystick Horizontal"},
        {PlayerAxis.CameraVertical, "Right Joystick Vertical"},
        {PlayerAxis.LeftTrigger, "Left Trigger"},
        {PlayerAxis.RightTrigger, "Right Trigger"},
        {PlayerAxis.UI_Horizontal, "Right Joystick Horizontal"},
        {PlayerAxis.UI_Vertical, "Right Joystick Vertical"},
    };

    public static Dictionary<PlayerAxis, string > mouseAxis = new Dictionary <PlayerAxis, string> {
        {PlayerAxis.MoveHorizontal, "Horizontal"},
        {PlayerAxis.MoveVertical, "Vertical"},
        {PlayerAxis.CameraHorizontal, "Mouse X"},
        {PlayerAxis.CameraVertical, "Mouse Y"},
        {PlayerAxis.UI_Horizontal, "Horizontal"},
        {PlayerAxis.UI_Vertical, "Vertical"},
    };

    private void Start()
    {
        ResetKeycodes();
        if (Input.GetJoystickNames().Length > 0 && Input.GetJoystickNames()[0]!="") inputMode = InputMode.both;
        else inputMode = InputMode.keyboard;
    }
    public static void ResetKeycodes () {
        playerActions[0].keyboardKey = KeyCode.E;
        playerActions[0].xboxKey = KeyCode.JoystickButton2;
        playerActions[1].keyboardKey = KeyCode.Space;
        playerActions[1].xboxKey = KeyCode.JoystickButton0;
        playerActions[2].keyboardKey = KeyCode.LeftControl;
        playerActions[2].xboxKey = KeyCode.JoystickButton1;
        playerActions[3].keyboardKey = KeyCode.LeftShift;
        playerActions[3].xboxKey = KeyCode.Joystick1Button8;
        playerActions[4].keyboardKey = KeyCode.F;
        playerActions[4].xboxKey = KeyCode.JoystickButton5;
        playerActions[5].keyboardKey = KeyCode.R;
        playerActions[5].xboxKey = KeyCode.JoystickButton3;
        playerActions[6].keyboardKey = KeyCode.Q;
        playerActions[6].xboxKey = KeyCode.JoystickButton4;
        playerActions[7].keyboardKey = KeyCode.Mouse0;
        playerActions[7].xboxKey = KeyCode.None;
        playerActions[8].keyboardKey = KeyCode.Mouse1;
        playerActions[8].xboxKey = KeyCode.None;
        playerActions[9].keyboardKey = KeyCode.Mouse0;
        playerActions[9].xboxKey = KeyCode.JoystickButton0;
        playerActions[10].keyboardKey = KeyCode.Escape;
        playerActions[10].xboxKey = KeyCode.JoystickButton1;
        playerActions[11].keyboardKey = KeyCode.Escape;
        playerActions[11].xboxKey = KeyCode.JoystickButton6;
        playerButtons[PlayerButton.Interact] = playerActions[0];
        playerButtons[PlayerButton.Jump] = playerActions[1];
        playerButtons[PlayerButton.Crouch] = playerActions[2];
        playerButtons[PlayerButton.Sprint] = playerActions[3];
        playerButtons[PlayerButton.Freeze] = playerActions[4];
        playerButtons[PlayerButton.Reverse] = playerActions[5];
        playerButtons[PlayerButton.Slow] = playerActions[6];
        playerButtons[PlayerButton.Global] = playerActions[7];
        playerButtons[PlayerButton.Local] = playerActions[8];
        playerButtons[PlayerButton.UI_Submit] = playerActions[9];
        playerButtons[PlayerButton.UI_Cancel] = playerActions[10];
        playerButtons[PlayerButton.Pause] = playerActions[11];
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