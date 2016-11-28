using UnityEngine;
using System.Collections;
using InControl;

namespace PlayerInput {

    public class MainActionSet : PlayerActionSet {

        // Directional mobility
        public PlayerAction Left;
        public PlayerAction Right;
        public PlayerAction Up;
        public PlayerAction Down;
        public PlayerTwoAxisAction Move;

        // Camera
        public PlayerAction CameraLeft;
        public PlayerAction CameraRight;
        public PlayerAction CameraUp;
        public PlayerAction CameraDown;
        public PlayerTwoAxisAction CameraMove;

        public MainActionSet() {
            // Directional mobility
            Left = CreatePlayerAction("Move Left");
            Right = CreatePlayerAction("Move Right");
            Up = CreatePlayerAction("Move Up");
            Down = CreatePlayerAction("Move Down");
            Move = CreateTwoAxisPlayerAction(Left, Right, Down, Up);
        }

        public virtual void AddKeyboardBindings() {
            // Directional mobility
            Up.AddDefaultBinding(Key.UpArrow);
            Down.AddDefaultBinding(Key.DownArrow);
            Left.AddDefaultBinding(Key.LeftArrow);
            Right.AddDefaultBinding(Key.RightArrow);
            Up.AddDefaultBinding(Key.Z);
            Down.AddDefaultBinding(Key.S);
            Left.AddDefaultBinding(Key.Q);
            Right.AddDefaultBinding(Key.D);
        }

        public virtual void AddGamePadBindings() {
            // Directional mobility
            Left.AddDefaultBinding(InputControlType.LeftStickLeft);
            Right.AddDefaultBinding(InputControlType.LeftStickRight);
            Up.AddDefaultBinding(InputControlType.LeftStickUp);
            Down.AddDefaultBinding(InputControlType.LeftStickDown);

        }

        public virtual void ResetBindings() {
            // Directional mobility
            Up.ResetBindings();
            Down.ResetBindings();
            Left.ResetBindings();
            Right.ResetBindings();
        }

    }

    public class KidActionSet : MainActionSet {

        // Jump
        public PlayerAction Jump;
        
        public KidActionSet() {
            // Jump
            Jump = CreatePlayerAction("Jump");
        }
    }

    public class RobotActionSet : MainActionSet {

        // Directional mobility
        public PlayerAction Grab;
        public PlayerAction Pull;
        public PlayerAction Bump;

        public RobotActionSet() {
            // Grab
            Grab = CreatePlayerAction("Grab");
            // Trow grab
            Pull = CreatePlayerAction("Pull");
            // Trow grab
            // Bump
            Bump = CreatePlayerAction("Bump");
        }
    }
}

