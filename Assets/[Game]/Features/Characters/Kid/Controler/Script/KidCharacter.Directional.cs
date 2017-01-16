using UnityEngine;
using System.Collections;
using PlayerInput;
using System.Collections.Generic;

namespace Gameplay {

    public partial class KidCharacter : Character {

        /*********
         * LOGIC *
         *********/

        private void Directional_Init() {
            directional.input = Vector2.zero;
        }

        private void Directional_LogicTick(float dt) {
            // Refresh directional logic
			directional.Tick(dt, Jump_IsJumping());
            
            // Apply to global velocity
            physic.globalVelocity += directional.velocity;
        }

        /**************
         * CONTROLLER *
         **************/
         
        private void Directional_ControllerTick(Controller input, float dt) {
            directional.input = input.shared.directional.Value;
        }



        /***************
         * DIRECTIONAL *
         ***************/


        /// <summary>
        /// Class containing everything necesary to make the kid jump and configure the parabolic of this jump.
        /// </summary>
        [System.Serializable]
        public class Directional {

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

				/// <summary>
				/// The speed the player move on both horizontal axis (x, z) when jumping.
				/// </summary>
				[Tooltip("The speed the player move on both horizontal axis (x, z) when jumping.")]
				public float moveSpeedWhileJumping = 15f;
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

			public void Tick(float dt, bool isPlayerJumping) {
				velocity = GetDirectionalVelocity(isPlayerJumping);
            }

			private Vector3 GetDirectionalVelocity(bool isPlayerJumping) 
			{
                // Directional
                Quaternion forwardRotation = directionalConfig.compassTransform.rotation;

                // Compute local direction
                Vector3 directional = Vector3.ClampMagnitude(new Vector3(input.x, 0, input.y), 1);

                // Convert direction to word
                directional = forwardRotation * directional;

                // Apply move speed to direction
				directional *= isPlayerJumping ? directionalConfig.moveSpeedWhileJumping : directionalConfig.moveSpeed;

                return directional;
            }

        }

        [Header("Directional")]
        [SerializeField]
        public Directional directional = new Directional();

    }

}