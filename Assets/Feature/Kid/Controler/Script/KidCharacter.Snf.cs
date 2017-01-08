using UnityEngine;
using System.Collections;
using PlayerInput;
using System.Collections.Generic;

namespace Gameplay
{

    public partial class KidCharacter : Character
    {

        /*********
         * LOGIC *
         *********/

        /// <summary>
        /// Initialise jump related logic.
        /// </summary>
        private void InitSignAndFeedback()
        {

        }

        /**********
         * PHYSIC *
         **********/

        /// <summary>
        /// Tick jump related logic.
        /// </summary>
        /// <param name="dt">Time elapsed since last tick.</param>
        private void SignAndFeebackLogicTick(float dt)
        {

        }

        /**************
         * CONTROLLER *
         **************/

        protected void SignAndFeebackRendering(float dt)
        {
            // Move speed
            signAndFeedback.SetMoveSpeed(directional.input.magnitude);

            // Rotation
            Quaternion characterRotation = characterCompass.transform.rotation;
            if(directional.input.magnitude > 0.1f)
            {
                characterRotation *= Quaternion.LookRotation(new Vector3(directional.input.x, 0, directional.input.y), Vector3.up);
            }
            //characterRotation *= Quaternion.Euler(0, 0, 90);
            signAndFeedback.SetRotation(characterRotation);
        }

        /*******************
         * SIGN & FEEDBACK *
         *******************/

        [Header("Sign and Feedback")]
        public KidCharacterSnf signAndFeedback;
        
    }

}