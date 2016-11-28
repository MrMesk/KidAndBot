using UnityEngine;
using System;
using System.Collections;
using InControl;

namespace PlayerInput {

    public abstract class SharedInput : PlayerActionSet {
        /** Directional **/
        // X Axis
        protected PlayerAction directionalRight;
        protected PlayerAction directionalLeft;
        // Y Axis
        protected PlayerAction directionalUp;
        protected PlayerAction directionalDown;
        // Two Axis
        public PlayerTwoAxisAction _directional;

        /** Camera **/
        // X Axis
        protected PlayerAction cameraRight;
        protected PlayerAction cameraLeft;
        // Y Axis
        protected PlayerAction cameraUp;
        protected PlayerAction cameraDown;
        // Two Axis
        public PlayerTwoAxisAction _camera;

        protected SharedInput() {
            // Directional
            directionalRight = CreatePlayerAction("directionalRight");
            directionalLeft = CreatePlayerAction("directionalLeft");
            directionalUp = CreatePlayerAction("directionalUp");
            directionalDown = CreatePlayerAction("directionalDown");
            _directional = CreateTwoAxisPlayerAction(directionalLeft, directionalRight, directionalDown, directionalUp);

            // Camera
            cameraRight = CreatePlayerAction("cameraRight");
            cameraLeft = CreatePlayerAction("cameraLeft");
            cameraUp = CreatePlayerAction("cameraUp");
            cameraDown = CreatePlayerAction("cameraDown");
            _camera = CreateTwoAxisPlayerAction(cameraLeft, cameraRight, cameraDown, cameraUp);
        }

        protected void Setup(
            SharedInputConfiguration sharedConfig,
            BindingFilter setupMode
        ) {
            // Directional
            InputHelper.AddDefaultBinding_DEPRECATED(directionalRight, sharedConfig.directional.xAxis.positive, setupMode);
            InputHelper.AddDefaultBinding_DEPRECATED(directionalLeft, sharedConfig.directional.xAxis.negative, setupMode);
            InputHelper.AddDefaultBinding_DEPRECATED(directionalUp, sharedConfig.directional.yAxis.positive, setupMode);
            InputHelper.AddDefaultBinding_DEPRECATED(directionalDown, sharedConfig.directional.yAxis.negative, setupMode);

            // Camera
            InputHelper.AddDefaultBinding_DEPRECATED(cameraRight, sharedConfig.camera.xAxis.positive, setupMode);
            InputHelper.AddDefaultBinding_DEPRECATED(cameraLeft, sharedConfig.camera.xAxis.negative, setupMode);
            InputHelper.AddDefaultBinding_DEPRECATED(directionalUp, sharedConfig.camera.yAxis.positive, setupMode);
            InputHelper.AddDefaultBinding_DEPRECATED(cameraDown, sharedConfig.camera.yAxis.negative, setupMode);

        }
    }
}