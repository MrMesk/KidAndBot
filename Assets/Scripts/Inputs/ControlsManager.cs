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
