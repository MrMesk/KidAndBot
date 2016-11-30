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
        public TriggerInput jump = new TriggerInput();
    }

    // Bot inputs
    [CreateAssetMenu(
        fileName = "botInputConfiguration",
        menuName = "Input Configuration/Bot",
        order = 0
        )]
    public class BotInputConfiguration : InputConfiguration {
        // Punch
        public TriggerInput punch;
        // Grab
        public TriggerInput grab;
        // Pull
        public TriggerInput pull;
        // Bump
        public TriggerInput bump;
    }

    [Serializable]
    public abstract class ConfigurableInput { }

    [Serializable]
    public class TriggerInput : ConfigurableInput {
        // Key input
        public Key keyboard = Key.None;
        // Gamepad input
        public InputControlType gamePad = InputControlType.None;
        // Mouse input
        public Mouse mouse = Mouse.None;
    }

    [Serializable]
    public class AxisInput : ConfigurableInput {
        // Positive
        public TriggerInput positive = new TriggerInput();
        // Negative
        public TriggerInput negative = new TriggerInput();
    }

    [Serializable]
    public class TwoAxisInput : ConfigurableInput {
        // X Axis
        public AxisInput xAxis = new AxisInput();
        // Y Axis
        public AxisInput yAxis = new AxisInput();
    }

}