using UnityEngine;
using System.Collections;
using PlayerInput;

namespace Gameplay
{

    public partial class KidCharacter : Character
    {
        /*********
         * UNITY *
         *********/

        public KidCompass compass;

        // Awake
        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();

            // Initialise this character
            Init();

        }

        // Gui
        private void OnGUI()
        {

            GUILayout.Label("Is grounded = " + IsGrounded());
        }

        /*****************
         * PHYSIC ENGINE *
         *****************/

        // Enter
        protected void OnCollisionEnter(Collision collision)
        {
            // Ignore collision if collided with self
            if (collision.gameObject == gameObject)
            {
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
            Climbing_OnCollisionEnter(collision);

            // Adherence
            Adherence_OnCollisionEnter(collision);
        }

        // Exit
        protected void OnCollisionExit(Collision collision)
        {
            // Ignore collision if collided with self
            if (collision.gameObject == gameObject)
            {
                return;
            }
            
            // Branch to sub-systems

            // Climbing
            Climbing_OnCollisionExit(collision);

            // Adherence
            Adherence_OnCollisionExit(collision);
        }

        // Exit
        protected void OnCollisionStay(Collision collision)
        {
            // Ignore collision if collided with self
            if (collision.gameObject == gameObject)
            {
                return;
            }

            // Branch to sub-systems

            // Climbing
            Climbing_OnCollisionStay(collision);

            // Adherence
            //Adherence_OnCollisionStay(collision);
        }


        /*********
         * LOGIC *
         *********/

        // Initialisation
        public void Init()
        {

            // Initialise controller (inputs)
            // Bind controller
            getController = delegate ()
            {
                return PlayerInput.Controller.kidController;
            };



            // Branch to sub-systems

            // Directinal
            Directional_Init();

            // Jump
            Jump_Init();

            // Climbing
            Climbing_Init();

        }

        // Logic tick
        protected override void LogicTick(float dt)
        {

            // Branch to sub-systems

            // Directional
            Directional_LogicTick(dt);

            // Jump
            Jump_LogicTick(dt);

            // Climbing 
            Climbing_LogicTick(dt);

            // Adherence
            Adherence_LogicTick(dt);

        }




        /**************
         * CONTROLLER *
         **************/

        /// <summary>
        /// Controller is called every frame. Use it for everything input related.
        /// </summary>
        protected override void ControllerTick(Controller input, float dt)
        {

            // Branch to sub-systems

            // Directional
            Directional_ControllerTick(input, dt);

            // Jump
            JumpController(input, dt);

            // Climbing
            Climbing_ControllerTick(input, dt);
        }


        /*************
         * RENDERING *
         *************/

        protected override void Rendering(float dt)
        {
            // SNF
            SignAndFeebackRendering(dt);
        }

    }

}