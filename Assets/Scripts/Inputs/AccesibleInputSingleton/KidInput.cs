using UnityEngine;
using System;
using System.Collections;
using InControl;

namespace PlayerInput {
    public class KidInput : SharedInput {
        /** Singleton setup **/
        private static KidInput instance = new KidInput();
        // Accesor
        public static KidInput GetInstance() { return instance; }

        /** Shared Accesors **/
        // Directional
        public static PlayerTwoAxisAction directional { get { return instance._directional; } }
        
        // Camera
        public static PlayerTwoAxisAction camera { get { return instance._camera; } }
        
        /** Jump **/
        private PlayerAction _jump;
        // Accesor
        public static PlayerAction jump { get { return GetInstance()._jump; } }

        /** Constructor **/
        protected KidInput() {
            // Jump
            _jump = CreatePlayerAction("jump");
        }

        /** Setup **/
        public void Setup(
            SharedInputConfiguration sharedConfig,
            KidInputConfiguration kidConfig,
            BindingFilter setupMode,
            InputDevice device = null,
            bool reset = false
        ) {
            // Reset ?
            if (reset) Reset();

            // Shared
            base.Setup(sharedConfig, setupMode);
            
            // Jump
            InputHelper.AddDefaultBinding_DEPRECATED(_jump, kidConfig.jump, setupMode);

            if(device != null) {
                Device = device;
                InputManager.OnDeviceAttached += InputManager_OnDeviceAttached;
                InputManager.OnDeviceDetached += InputManager_OnDeviceDetached;
            }
        }

        private void InputManager_OnDeviceAttached(InputDevice obj) {
            if (Device == null) {
                Device = obj;
                Debug.Log("KID CONNECTED !");
            }
        }

        private void InputManager_OnDeviceDetached(InputDevice obj) {
            if(Device == obj) {
                Device = null;
                Debug.Log("KID DISCONECTED !");
            }
        }        
    }

}