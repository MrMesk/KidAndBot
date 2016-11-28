using UnityEngine;
using System;
using System.Collections;
using InControl;
using PlayerInput;

public class ControlsManager : MonoBehaviour {
   
    public static ControlsManager instance;

    // Configuration
    [Header("Configuration files")]
    public SharedInputConfiguration sharedInputsConfig;
    public KidInputConfiguration kidInputsConfig;
    public BotInputConfiguration botInputsConfig;
    
    // Setup mode
    /*
    public enum ContolsSetupMode {
        NONE,
        KEYBOARD_GAMEPAD,
        GAMEPAD_KEYBOARD,
        GAMEPAD_GAMEPAD
    }
    [Header("Setup mode")]
    [SerializeField]
    private ContolsSetupMode controlsSetupModep = ContolsSetupMode.GAMEPAD_KEYBOARD;
    public static ContolsSetupMode GetContolsSetupMode() { return instance.controlsSetupModep; }
    */
    /*
    // Initialisation
    private void Initialise() {
        const int kidId = 0;
        const int botId = 1;
        switch (controlsSetupModep) {
            case ContolsSetupMode.KEYBOARD_GAMEPAD:
                KidInput.GetInstance().Setup(sharedInputsConfig, kidInputsConfig, SetupMode.KEYBOARD);
                BotInput.GetInstance().Setup(sharedInputsConfig, botInputsConfig, SetupMode.GAMEPAD);
                break;
            case ContolsSetupMode.GAMEPAD_KEYBOARD:
                KidInput.GetInstance().Setup(
                    sharedInputsConfig,
                    kidInputsConfig,
                    SetupMode.GAMEPAD,
                    (InputManager.Devices.Count > kidId) ? InputManager.Devices[kidId] : null
                    );
                BotInput.GetInstance().Setup(sharedInputsConfig, botInputsConfig, SetupMode.KEYBOARD);
                break;
            case ContolsSetupMode.GAMEPAD_GAMEPAD:
                KidInput.GetInstance().Setup(
                    sharedInputsConfig,
                    kidInputsConfig,
                    SetupMode.GAMEPAD,
                    (InputManager.Devices.Count > kidId) ? InputManager.Devices[kidId] : null
                    );
                BotInput.GetInstance().Setup(
                    sharedInputsConfig,
                    botInputsConfig,
                    SetupMode.GAMEPAD,
                    (InputManager.Devices.Count > botId) ? InputManager.Devices[botId] : null
                    );
                break;
        }

    }
    */

    // Debug
    public void OnGUI() {
        GUILayout.Label(InputManager.Devices.Count.ToString());
        GUILayout.Label("Bot bump = " + BotInput.bump.IsPressed.ToString());
        GUILayout.Label("Kid jump = " + KidInput.jump.IsPressed.ToString());
    }

    private void Awake() {
        // Singleton setup
        if (instance == null) {
            instance = this;
        } else {

        }

        Initialise();
    }

    private void Initialise() {
        Controller.playerOneController = new Controller();
        Controller.playerTwoController = new Controller();
        Controller.kidController = Controller.playerOneController;
        Controller.botController = Controller.playerTwoController;

        HandleDeviceCountChange();
    }

    public void HandleDeviceCountChange() {
        var devices = InputManager.Devices;
        switch (devices.Count) {
            case 0:
                // Solo player
                throw new NotImplementedException();
            case 1:
                // Two players (Keyboard + GamePad)
                // Gamepad
                SetupControllerForGamePad(Controller.kidController, devices[0]);
                // Keyboard
                SetupControllerForKeybord(Controller.botController);
                break;
            case 2:
                // Two players (GamePad + GamePad)
                throw new NotImplementedException();
            default:
                throw new NotImplementedException();
        }
    }

    public void SetupControllerForGamePad(Controller controller, InputDevice device) {
        // Shared
        PrepareActionSetForGamePad(controller.shared, device);
        controller.shared.AddDefaultBinding(sharedInputsConfig, BindingFilter.GAMEPAD);
        // Kid
        PrepareActionSetForGamePad(controller.kid, device);
        controller.kid.AddDefaultBinding(kidInputsConfig, BindingFilter.GAMEPAD);
        // Bot
        PrepareActionSetForGamePad(controller.bot, device);
        controller.bot.AddDefaultBinding(botInputsConfig, BindingFilter.GAMEPAD);
    }

    public void PrepareActionSetForGamePad(PlayerActionSet playerActionSet, InputDevice device) {
        foreach (var action in playerActionSet.Actions) {
            action.ClearBindings();
        }
        playerActionSet.Device = device;
        playerActionSet.ListenOptions.IncludeControllers = true;
        playerActionSet.ListenOptions.IncludeKeys = false;
        playerActionSet.ListenOptions.IncludeMouseButtons = false;
    }

    public void SetupControllerForKeybord(Controller controller) {
        // Shared
        PrepareActionSetForKeybord(controller.shared);
        controller.shared.AddDefaultBinding(sharedInputsConfig, BindingFilter.KEYBOARD);
        // Kid
        PrepareActionSetForKeybord(controller.kid);
        controller.kid.AddDefaultBinding(kidInputsConfig, BindingFilter.KEYBOARD);
        // Bot
        PrepareActionSetForKeybord(controller.bot);
        controller.bot.AddDefaultBinding(botInputsConfig, BindingFilter.KEYBOARD);
    }

    public void PrepareActionSetForKeybord(PlayerActionSet playerActionSet) {
        foreach (var action in playerActionSet.Actions) {
            action.ClearBindings();
        }
        playerActionSet.Device = null;
        playerActionSet.ListenOptions.IncludeControllers = false;
        playerActionSet.ListenOptions.IncludeKeys = true;
        playerActionSet.ListenOptions.IncludeMouseButtons = true;
    }

}
