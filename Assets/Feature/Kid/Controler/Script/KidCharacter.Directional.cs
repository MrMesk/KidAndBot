using UnityEngine;
using System.Collections;
using PlayerInput;
using System.Collections.Generic;

namespace Gameplay {

    public partial class KidCharacter : Character {

        /*********
         * LOGIC *
         *********/

        private void InitDirectional() {
            directional.input = Vector2.zero;
        }

        private void DirectionalLogicTick(float dt) {
            // Refresh directional logic
            directional.Tick(dt);

            // Apply to global velocity
            physic.globalVelocity += directional.velocity;
        }

        /**************
         * CONTROLLER *
         **************/
         
        private void DirectionalController(Controller input, float dt) {
            directional.input = input.shared.directional.Value;
        }



        /***************
         * DIRECTIONAL *
         ***************/


        /// <summary>
        /// Class containing everything necesary to make the kid jump and configure the parabolic of this jump.
        /// </summary>
        [System.Serializable]
        private class Directional {

            // Configuration

            /// <summary>
            /// Container of jump configuration related data.
            /// </summary>
            [System.Serializable]
            public class DirectionalConfig {
                /// <summary>
                /// The compass used to transpose the stick input to world directional translation.
                /// </summary>
                [Tooltip("The compass used to transpose the stick input to world directional translation.")]
                public Transform compassTransform;
                
                /// <summary>
                /// The speed the player move on both horizontal axis (x, z).
                /// </summary>
                [Tooltip("The speed the player move on both horizontal axis (x, z).")]
                public float moveSpeed = 7.5f;
            }

            /// <summary>
            /// Jump configuration related data.
            /// </summary>
            [SerializeField]
            public DirectionalConfig directionalConfig = new DirectionalConfig();

            // Player left stick input (local)
            [System.NonSerialized]
            public Vector2 input;
            
            // Directional velocity (world)
            [System.NonSerialized]
            public Vector3 velocity;

            public void Tick(float dt) {
                velocity = GetDirectionalVelocity();
            }

            private Vector3 GetDirectionalVelocity() {
                // Directional
                Quaternion forwardRotation = directionalConfig.compassTransform.rotation;

                // Compute local direction
                Vector3 directional = Vector3.ClampMagnitude(new Vector3(input.x, 0, input.y), 1);

                // Convert direction to word
                directional = forwardRotation * directional;

                // Apply move speed to direction
                directional *= directionalConfig.moveSpeed;

                return directional;
            }

        }

        [Header("Directional")]
        [SerializeField]
        private Directional directional = new Directional();

    }

}