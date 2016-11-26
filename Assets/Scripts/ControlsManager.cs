using UnityEngine;
using System;
using System.Collections;
using InControl;
using PlayerInput;

public class ControlsManager : MonoBehaviour {
   
    public static ControlsManager instance;
    
    public enum ContolsSetupMode {
        NONE,
        KEYBOARD_CONTROLLER,
        CONTROLLER_CONTROLLER
    }
    
    [SerializeField] private ContolsSetupMode controlsSetup;
    
    public static KidActionSet kidInput;
    public static RobotActionSet robotInput;

    private void Awake() {
        Initialise();
    }

    // Initialisation
    private void Initialise() {
        kidInput = new KidActionSet();
        robotInput = new RobotActionSet();

        switch (controlsSetup) {
            case ContolsSetupMode.KEYBOARD_CONTROLLER:
                kidInput.AddGamePadBindings();
                robotInput.AddKeyboardBindings();
                break;
            case ContolsSetupMode.CONTROLLER_CONTROLLER:
                kidInput.AddGamePadBindings();
                robotInput.AddGamePadBindings();
                break;
            default:
                throw new NotImplementedException();
        }
    }

    // Get / Set
    public static ContolsSetupMode GetContolsSetupMode() {
        return instance.controlsSetup;
    }

    public void Update() {

    }

    public void OnGUI() {
        GUILayout.Label(InputManager.Devices.Count.ToString());
        GUILayout.Label(robotInput.Up.IsPressed.ToString());
    }

}
