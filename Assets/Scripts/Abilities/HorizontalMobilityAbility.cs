using UnityEngine;
using System.Collections;
using System;
using InControl;

namespace Abilities {


    public partial class HorizontalMobilityAbility {
        public class PlayerActions : PlayerActionSet {
            public PlayerAction Left;
            public PlayerAction Right;
            public PlayerAction Up;
            public PlayerAction Down;
            public PlayerTwoAxisAction Move;

            public PlayerActions() {
                Left = CreatePlayerAction("Move Left");
                Right = CreatePlayerAction("Move Right");
                Up = CreatePlayerAction("Move Up");
                Down = CreatePlayerAction("Move Down");
                Move = CreateTwoAxisPlayerAction(Left, Right, Down, Up);
            }
        }
    }

    public partial class HorizontalMobilityAbility : MonoBehaviour {
        // Configuration
        [Header("Configuration")]
        public float _moveSpeed = 7.5f;

        public enum DebugInputMode {
            NONE,
            GAMEPAD,
            KEYBOARD,
            BOTH
        }


        // Debug
        [Header("Debug")]
        public DebugInputMode debugInputMode = DebugInputMode.BOTH;

        // S&F
        [Header("Sings & Feedbacks")]
        [Range(0,1)]
        public float editLookDirectionDeadZone = 0.1f;


        // State
        [NonSerialized] private Character character;
        [NonSerialized] private Vector2 directionalInput;
        // Input
        [NonSerialized] public PlayerActions playerInput;

        public Vector3 directionalVelocity { get; private set; }

        private void Start() {
            // Retrieve required component(s)
            character = GetComponentInParent<Character>();
            if (character == null) {
                this.enabled = false;
                return;
            }
            Initialise();
        }

        private void Initialise() {
            playerInput = new PlayerActions();


        }

        private void InitialiseInput() {
            playerInput = new PlayerActions();

            playerInput.Up.AddDefaultBinding(Key.UpArrow);
            playerInput.Down.AddDefaultBinding(Key.DownArrow);
            playerInput.Left.AddDefaultBinding(Key.LeftArrow);
            playerInput.Right.AddDefaultBinding(Key.RightArrow);

            playerInput.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
            playerInput.Right.AddDefaultBinding(InputControlType.LeftStickRight);
            playerInput.Up.AddDefaultBinding(InputControlType.LeftStickUp);
            playerInput.Down.AddDefaultBinding(InputControlType.LeftStickDown);

            playerInput.ListenOptions.MaxAllowedBindings = 2;

        }

        public void Update() {
            // Input
            switch (debugInputMode) {
                case DebugInputMode.GAMEPAD:
                    directionalInput = new Vector2(Input.GetAxisRaw("Left Stick X"), Input.GetAxisRaw("Left Stick Y"));
                    break;
                case DebugInputMode.KEYBOARD:
                    directionalInput = new Vector2(Input.GetAxisRaw("Horizontal Keyboard"), Input.GetAxisRaw("Vertical Keyboard"));
                    break;
                case DebugInputMode.NONE:
                    directionalInput = Vector2.zero;
                    break;
                case DebugInputMode.BOTH:
                default:
                    directionalInput = Vector2.Lerp(
                        new Vector2(Input.GetAxisRaw("Left Stick X"), Input.GetAxisRaw("Left Stick Y")),
                        new Vector2(Input.GetAxisRaw("Horizontal Keyboard"), Input.GetAxisRaw("Vertical Keyboard")),
                        0.5f
                        );
                    break;
            }
            
        }

        public void LateUpdate() {
            // Refresh look rotation
            if (directionalVelocity.magnitude > editLookDirectionDeadZone) {
                character.lookRotation = Quaternion.LookRotation(directionalVelocity, -character.gravity);
            }
        }

        private void FixedUpdate() {
            // TODO : True climbing ability
            // Do not walk if climbing
            //if (character.IsClimbing())
            //    return;

            // Do not walk if grabing
            if (character.IsGrabbing())
                return;

            // Directional
            directionalVelocity = GetDirectionalVelocity();
            
            // Apply to global velocity
            character.globalVelocity += directionalVelocity;
        }

        protected Vector3 GetDirectionalVelocity() {
            // Directional
            Quaternion forwardRotation = character.characterCompass.transform.rotation;
            Vector3 directional = Vector3.ClampMagnitude(new Vector3(directionalInput.x, 0, directionalInput.y), 1);
            directional = forwardRotation * directional;
            directional *= _moveSpeed;
            return directional;
        }
    }
}

