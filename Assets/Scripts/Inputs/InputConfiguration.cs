using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using InControl;

namespace PlayerInput {
    
    public abstract class InputConfiguration : ScriptableObject { }

    // Shared inputs
    [CreateAssetMenu(
        fileName = "sharedInputConfiguration",
        menuName = "Input Configuration/Shared",
        order = 0
        )]
    public class SharedInputConfiguration : InputConfiguration {
        // Directional
        public TwoAxisInput directional = new TwoAxisInput();
        // Camera
        public TwoAxisInput camera = new TwoAxisInput();
    }

    // Kid inputs
    [CreateAssetMenu(
        fileName = "kidInputConfiguration",
        menuName = "Input Configuration/Kid",
        order = 0
        )]
    public class KidInputConfiguration : InputConfiguration {
        // Jump
        public ButtonInput jump = new ButtonInput();
    }

    // Bot inputs
    [CreateAssetMenu(
        fileName = "botInputConfiguration",
        menuName = "Input Configuration/Bot",
        order = 0
        )]
    public class BotInputConfiguration : InputConfiguration {
        // Punch
        public ButtonInput punch;
        // Grab
        public ButtonInput grab;
        // Pull
        public ButtonInput pull;
        // Bump
        public ButtonInput bump;
    }

    [Serializable]
    public abstract class Input { }

    [Serializable]
    public class ButtonInput : Input {
        // Key input
        public Key keyboard = Key.None;
        // Gamepad input
        public InputControlType gamePad = InputControlType.None;
        // Mouse input
        public Mouse mouse = Mouse.None;
    }

    [Serializable]
    public class AxisInput : Input {
        // Positive
        public ButtonInput positive = new ButtonInput();
        // Negative
        public ButtonInput negative = new ButtonInput();
    }

    [Serializable]
    public class TwoAxisInput : Input {
        // X Axis
        public AxisInput xAxis = new AxisInput();
        // Y Axis
        public AxisInput yAxis = new AxisInput();
    }

}