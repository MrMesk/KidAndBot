using UnityEngine;
using System.Collections;
using System;

namespace Abilities {
    public class HorizontalMobilityAbility : MonoBehaviour {
        [Header("Configuration")]
        public float _moveSpeed = 7.5f;

        public enum DebugInputMode {
            NONE,
            GAMEPAD,
            KEYBOARD,
            BOTH
        }
        [Header("Debug")]
        public DebugInputMode debugInputMode = DebugInputMode.BOTH;

       [Header("Sings & Feedbacks")]
        [Range(0,1)]
        public float editLookDirectionDeadZone = 0.1f;

        [NonSerialized] private Character character;
        [NonSerialized] private Vector2 directionalInput;

        public Vector3 directionalVelocity { get; private set; }

        private void Start() {
            // Retrieve required component(s)
            character = GetComponentInParent<Character>();
            if (character == null) {
                this.enabled = false;
                return;
            }
        }

        public void Update() {
            // Input
            switch (debugInputMode) {
                case DebugInputMode.GAMEPAD:
                    directionalInput = new Vector2(Input.GetAxisRaw("Left Stick X"), Input.GetAxisRaw("Left Stick Y"));
                    break;
                case DebugInputMode.BOTH:
                default:
                    directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
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

