using UnityEngine;
using System.Collections;
using PlayerInput;

namespace Gameplay {

    public partial class KidCharacter : Character {

        /*********
         * UNITY *
         *********/

        // Awake
        /// <inheritdoc />
        protected override void Awake() {
            base.Awake();

            // Initialise this character
            Init();

        }

        // Gui
        private void OnGUI() {
            
            GUILayout.Label("Is grounded = " + IsGrounded());
        }

        /*****************
         * PHYSIC ENGINE *
         *****************/

        // Enter
        protected void OnCollisionEnter(Collision collision) {
            // Ignore collision if collided with self
            if (collision.gameObject == gameObject) {
                return;
            }

            // TODO :
            // Refresh current parrent
            // Connected collider
            //bool needParentRefresh = false;
            //if (!connectedColliders.Contains(collision.collider)) {
            //    connectedColliders.Add(collision.collider);
            //    needParentRefresh = true;
            //}



            // Branch to sub-systems

            // Climbing
            OnCollisionEnterClimbing(collision);
        }

        // Exit
        protected void OnCollisionExit(Collision collision) {
            // Ignore collision if collided with self
            if (collision.gameObject == gameObject) {
                return;
            }

            // TODO :
            // Refresh current parrent



            // Branch to sub-systems

            // Climbing
            OnCollisionExitClimbing(collision);
        }


        /*********
         * LOGIC *
         *********/

        // Initialisation
        public void Init() {

            // Initialise controller (inputs)
            // Bind controller
            getController = delegate () {
                return PlayerInput.Controller.kidController;
            };



            // Branch to sub-systems

            // Directinal
            InitDirectional();

            // Jump
            InitJump();

            // Climbing
            InitClimbing();

        }

        // Logic tick
        protected override void LogicTick(float dt) {
            
            // Branch to sub-systems

            // Directional
            DirectionalLogicTick(dt);

            // Jump
            JumpLogicTick(dt);

            // Climbing 
            ClimbingLogicTick(dt);

        }




        /**************
         * CONTROLLER *
         **************/

        /// <summary>
        /// Controller is called every frame. Use it for everything input related.
        /// </summary>
        protected override void ControllerTick(Controller input, float dt) {

            // Branch to sub-systems

            // Directional
            DirectionalController(input, dt);

            // Jump
            JumpController(input, dt);

            // Climbing
            ClimbingControllerTick(input, dt);
        }
        
    }

}