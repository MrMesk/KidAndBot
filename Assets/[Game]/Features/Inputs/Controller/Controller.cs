using UnityEngine;
using System.Collections;
using InControl;

namespace PlayerInput {
    
    public class Controller
	{
        public static Controller kidController;
        public static Controller botController;
        public static Controller playerOneController;
        public static Controller playerTwoController;

        public SharedControls shared;
        public KidControls kid;
        public BotControls bot;

        public InputDevice device = null;

        public Controller()
		{
            shared = new SharedControls();
            kid = new KidControls();
            bot = new BotControls();
        }

        public static void Swap()
		{
            Controller temp = Controller.kidController;
            Controller.kidController = Controller.botController;
            Controller.botController = temp;
        }
    }
    
    public class SharedControls : PlayerActionSet
	{
        /** Directional **/
        // X Axis
        protected PlayerAction directionalRight;
        protected PlayerAction directionalLeft;
        // Y Axis
        protected PlayerAction directionalUp;
        protected PlayerAction directionalDown;
        // Two Axis
        public PlayerTwoAxisAction directional;

        /** Camera **/
        // X Axis
        protected PlayerAction cameraRight;
        protected PlayerAction cameraLeft;
        // Y Axis
        protected PlayerAction cameraUp;
        protected PlayerAction cameraDown;
        // Two Axis
        public PlayerTwoAxisAction camera;
        
        public SharedControls()
		{
            /** Shared **/
            // Directional
            directionalRight = CreatePlayerAction("directionalRight");
            directionalLeft = CreatePlayerAction("directionalLeft");
            directionalUp = CreatePlayerAction("directionalUp");
            directionalDown = CreatePlayerAction("directionalDown");
            directional = CreateTwoAxisPlayerAction(directionalLeft, directionalRight, directionalDown, directionalUp);
            
            // Camera
            cameraRight = CreatePlayerAction("cameraRight");
            cameraLeft = CreatePlayerAction("cameraLeft");
            cameraUp = CreatePlayerAction("cameraUp");
            cameraDown = CreatePlayerAction("cameraDown");
            camera = CreateTwoAxisPlayerAction(cameraLeft, cameraRight, cameraDown, cameraUp);
        }

        public void AddDefaultBinding(PlayerInput.Configuration.Shared sharedInputsConfig, BindingFilter bindingFilter)
		{
            // Directional
            InputHelper.AddDefaultBinding(directionalRight, sharedInputsConfig.directional.xAxis.positive, bindingFilter);
            InputHelper.AddDefaultBinding(directionalLeft, sharedInputsConfig.directional.xAxis.negative, bindingFilter);
            InputHelper.AddDefaultBinding(directionalUp, sharedInputsConfig.directional.yAxis.positive, bindingFilter);
            InputHelper.AddDefaultBinding(directionalDown, sharedInputsConfig.directional.yAxis.negative, bindingFilter);

            // Camera
            InputHelper.AddDefaultBinding(cameraRight, sharedInputsConfig.camera.xAxis.positive, bindingFilter);
            InputHelper.AddDefaultBinding(cameraLeft, sharedInputsConfig.camera.xAxis.negative, bindingFilter);
            InputHelper.AddDefaultBinding(cameraUp, sharedInputsConfig.camera.yAxis.positive, bindingFilter);
            InputHelper.AddDefaultBinding(cameraDown, sharedInputsConfig.camera.yAxis.negative, bindingFilter);
        }

    }

    public class KidControls : PlayerActionSet
	{
        /** Jump **/
        public PlayerAction jump;
		public PlayerAction anchor;

		public KidControls()
		{
            /** Kid **/
            // Jump
            jump = CreatePlayerAction("jump");
			// Anchor
			anchor = CreatePlayerAction("anchor");

		}

        public void AddDefaultBinding(PlayerInput.Configuration.Kid kidInputsConfig, BindingFilter bindingFilter)
		{
            // Jump
            InputHelper.AddDefaultBinding(jump, kidInputsConfig.jump, bindingFilter);
			InputHelper.AddDefaultBinding(anchor, kidInputsConfig.anchor, bindingFilter);
		}
    }

    public class BotControls : PlayerActionSet {

        /** Punch **/
        public PlayerAction punch;
        /** Grab **/
        public PlayerAction grab;
        /** Pull **/
        public PlayerAction pull;
        /** Bump **/
        public PlayerAction bump;

        public BotControls() {
            /** Bot **/
            // Punch
            punch = CreatePlayerAction("punch");
            // Grab
            grab = CreatePlayerAction("grab");
            // Pull
            pull = CreatePlayerAction("pull");
            // Bump
            bump = CreatePlayerAction("bump");
        }

        public void AddDefaultBinding(PlayerInput.Configuration.Bot botInputsConfig, BindingFilter bindingFilter) {
            // Punch
            InputHelper.AddDefaultBinding(punch, botInputsConfig.punch, bindingFilter);
            // Grab
            InputHelper.AddDefaultBinding(grab, botInputsConfig.grab, bindingFilter);
            // Pull
            InputHelper.AddDefaultBinding(pull, botInputsConfig.pull, bindingFilter);
            // Bump
            InputHelper.AddDefaultBinding(bump, botInputsConfig.bump, bindingFilter);
        }
    }
}