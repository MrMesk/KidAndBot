using UnityEngine;
using System.Collections;
using InControl;

namespace PlayerInput {
    public class BotInput : SharedInput {
        // Singleton setup
        private static BotInput instance = new BotInput();
        // Accesor
        public static BotInput GetInstance() { return instance; }


        /** Shared Accesors **/
        // Directional
        public static PlayerTwoAxisAction directional { get { return instance._directional; } }

        // Camera
        public static PlayerTwoAxisAction camera { get { return instance._camera; } }

        /** Punch **/
        public static PlayerAction punch;
        /** Grab **/
        public static PlayerAction grab;
        /** Pull **/
        public static PlayerAction pull;
        /** Bump **/
        private PlayerAction _bump;
        public static PlayerAction bump { get { return instance._bump; } }

        /** Constructor **/
        private BotInput() {
            // Punch
            punch = CreatePlayerAction("punch");
            // Grab
            grab = CreatePlayerAction("grab");
            // Pull
            pull = CreatePlayerAction("pull");
            // Bump
            _bump = CreatePlayerAction("bump");
        }

        /** Setup **/
        public void Setup(
            SharedInputConfiguration sharedConfig,
            BotInputConfiguration botConfig,
            BindingFilter setupMode,
            InputDevice device = null,
            bool reset = false
        ) {
            // Reset ?
            if (reset) Reset();

            // Shared
            base.Setup(sharedConfig, setupMode);

            // Punch
            InputHelper.AddDefaultBinding_DEPRECATED(punch, botConfig.punch, setupMode);
            // Grab
            InputHelper.AddDefaultBinding_DEPRECATED(grab, botConfig.grab, setupMode);
            // Pull
            InputHelper.AddDefaultBinding_DEPRECATED(pull, botConfig.pull, setupMode);
            // Bump
            InputHelper.AddDefaultBinding_DEPRECATED(_bump, botConfig.bump, setupMode);
        }
    }
}