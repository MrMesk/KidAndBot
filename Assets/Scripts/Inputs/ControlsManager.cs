using UnityEngine;
using System;
using System.Collections;
using InControl;
using PlayerInput;

public class ControlsManager : MonoBehaviour {
   
    public static ControlsManager instance;

    // Configuration
    [Header("Configuration files")]
    public PlayerInput.Configuration.Shared sharedInputsConfig;
    public PlayerInput.Configuration.Kid kidInputsConfig;
    public PlayerInput.Configuration.Bot botInputsConfig;
    
    private void Awake() {
        // Singleton setup
        if (instance == null) {
            instance = this;
        } else {

        }

        //sharedInputsConfig = ScriptableObject.CreateInstance<SharedInputConfiguration>();

        //sharedInputsConfig.camera.xAxis.positive.gamePad = InputControlType.RightStickRight;
        //sharedInputsConfig.camera.xAxis.negative.gamePad = InputControlType.RightStickLeft;
        //sharedInputsConfig.camera.yAxis.positive.gamePad = InputControlType.RightStickUp;
        //sharedInputsConfig.camera.yAxis.negative.gamePad = InputControlType.RightStickDown;
        
        //

        Initialise();
    }

    private void Initialise() {
        Controller.playerOneController = new Controller();
        Controller.playerTwoController = new Controller();
        Controller.kidController = Controller.playerOneController;
        Controller.botController = Controller.playerTwoController;

        InputManager.OnDeviceAttached += InputManager_OnDeviceAttached;
        InputManager.OnDeviceDetached += InputManager_OnDeviceDetached;
                

        HandleDeviceCountChange();
    }

    private void InputManager_OnDeviceAttached(InputDevice device) {
        if (Controller.kidController.device == null) {
            // Kid controller attached
            Debug.Log("Kid controller attached - " + device.SortOrder);
            SetupControllerForGamePad(Controller.kidController, device);
        } else
        if (Controller.botController.device == null) {
            // Bot controller attached
            Debug.Log("Bot controller attached - " + device.SortOrder);
            SetupControllerForGamePad(Controller.botController, device);
        }
    }

    private void InputManager_OnDeviceDetached(InputDevice device) {
        // Device detached
        if (
            Controller.kidController.device != null &&
            Controller.kidController.device.SortOrder == device.SortOrder
        ) {
            // Kid controller detached
            Debug.Log("Kid controller detached - " + device.SortOrder);
            SetupControllerForKeybord(Controller.kidController);
        }
        else
        if (
            Controller.kidController.device != null && 
            Controller.botController.device.SortOrder == device.SortOrder
        ) {
            // Bot controller detached
            Debug.Log("Bot controller detached - " + device.SortOrder);
            SetupControllerForKeybord(Controller.botController);
        }
    }

    public void HandleDeviceCountChange() {
        var devices = InputManager.Devices;
        switch (devices.Count) {
            case 0:
                // Solo player
                break;
            case 1:
                // Two players (Keyboard + GamePad)
                // Gamepad
                SetupControllerForGamePad(Controller.kidController, devices[0]);
                // Keyboard
                SetupControllerForKeybord(Controller.botController);
                break;
            case 2:
                // Two players (GamePad + GamePad)
                // Gamepad
                SetupControllerForGamePad(Controller.kidController, devices[0]);
                // Keyboard
                SetupControllerForGamePad(Controller.botController, devices[1]);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public void SetupControllerForGamePad(Controller controller, InputDevice device) {
        // Attach device
        controller.device = device;
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
        // Detach device
        controller.device = null;
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
