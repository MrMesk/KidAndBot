using UnityEngine;
using System;
using System.Collections;
using InControl;

namespace PlayerInput {
    public enum BindingFilter {
        NONE,
        KEYBOARD,
        GAMEPAD,
        BOTH
    }
    
    public class InputHelper : MonoBehaviour {
        [Obsolete]
        public static void AddDefaultBinding_DEPRECATED(PlayerAction playerAction, ButtonInput input, BindingFilter setupMode) {
            switch (setupMode) {
                case BindingFilter.KEYBOARD:
                    // Key
                    if (input.keyboard != Key.None)
                        playerAction.AddDefaultBinding(input.keyboard);
                    // Mouse
                    if (input.mouse != Mouse.None)
                        playerAction.AddDefaultBinding(input.mouse);
                    break;
                case BindingFilter.GAMEPAD:
                    // Input control type
                    if (input.gamePad != InputControlType.None)
                        playerAction.AddDefaultBinding(input.gamePad);
                    break;
                case BindingFilter.BOTH:
                    // Key
                    if (input.keyboard != Key.None)
                        playerAction.AddDefaultBinding(input.keyboard);
                    // Mouse
                    if (input.mouse != Mouse.None)
                        playerAction.AddDefaultBinding(input.mouse);
                    // Input control type
                    if (input.gamePad != InputControlType.None)
                        playerAction.AddDefaultBinding(input.gamePad);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }


        public static void AddDefaultBinding(PlayerAction playerAction, ButtonInput input, BindingFilter setupMode) {
            switch (setupMode) {
                case BindingFilter.KEYBOARD:
                    // Key
                    if (input.keyboard != Key.None)
                        playerAction.AddDefaultBinding(input.keyboard);
                    // Mouse
                    if (input.mouse != Mouse.None)
                        playerAction.AddDefaultBinding(input.mouse);
                    break;
                case BindingFilter.GAMEPAD:
                    // Input control type
                    if (input.gamePad != InputControlType.None)
                        playerAction.AddDefaultBinding(input.gamePad);
                    break;
                case BindingFilter.BOTH:
                    // Key
                    if (input.keyboard != Key.None)
                        playerAction.AddDefaultBinding(input.keyboard);
                    // Mouse
                    if (input.mouse != Mouse.None)
                        playerAction.AddDefaultBinding(input.mouse);
                    // Input control type
                    if (input.gamePad != InputControlType.None)
                        playerAction.AddDefaultBinding(input.gamePad);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

    }
}