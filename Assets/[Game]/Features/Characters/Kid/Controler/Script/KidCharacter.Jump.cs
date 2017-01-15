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
        private void Jump_Init()
        {
            // *** Bind deletates ***
            // End active jump when the character hit the ground
            physic.eHitGround += Jump_EndActiveJump;

            // End active jump when the character connect to a new climbable object
            climbing.eAttachedToANewClimbalbeObject += Jump_EndActiveJump;

            // *** Init regular gravity ***
            /// Explanation :
            /// The gravity is dynamic in the game.
            /// It's values are regulated by both how the player's jump is configurated and the state of the jump input.
            /// So even if the player isn't perdorming any jump, the system still need a jump object only to apply gravity.
            // Set the regular jump as the one whose gravity will be used when the player isn't perfoming any jump.
            gravityJumpFallback = regularJump;
            // Initialise the gravity of this jump.
            gravityJumpFallback.RefreshGravityAcceleration(false);
        }

        /// <summary>
        /// End any jump the kid is currently performing, as well as resetting the gravity.
        /// </summary>
        private void Jump_EndActiveJump()
        {
            // Reset active jump
            if (activeJump != null)
            {
                activeJump.EndJump();
                activeJump = null;
            }
            // Reset active gravity
            if (gravityJumpFallback != null)
            {
                gravityJumpFallback.EndJump();
            }
        }

        /// <summary>
        /// Returns if the kid is currently jumping.
        /// </summary>
        /// <returns></returns>
        public bool Jump_IsJumping()
        {
            return activeJump != null;
        }

        /**********
         * PHYSIC *
         **********/

        /// <summary>
        /// Tick jump related logic.
        /// </summary>
        /// <param name="dt">Time elapsed since last tick.</param>
        private void Jump_LogicTick(float dt)
        {

            // Normaly apply jump logic,
            if (!climbing.IsClimbing())
            {
                // Tf the character is jumping,
                if (Jump_IsJumping())
                {
                    // Then apply this jump's physic.
                    
                    // LEGACy :
                    //Vector3 jumpVelocity = activeJump.velocity;
                    // Apply
                    //physic.globalVelocity += jumpVelocity;

                    // Position (p) :
                    //
                    //            at²
                    // p += vt + -----
                    //             2
                    //
                    // Change the global velocity sytem to a translation system
                    Vector3 translation = activeJump.velocity * dt + 0.5f * activeJump.acceleration * dt * dt;
                    physic.translate += translation;

                    // Tick active jump
                    activeJump.Tick(dt);
                }
                else if(!attach.IsAttached())
                // If he ain't juming an not attached,
                {
                    // Then apply the gravity physic.

                    // Tick garavity (DEBUG : before)
                    gravityJumpFallback.Tick(dt);

                    //Vector3 jumpVelocity = gravityJumpFallback.velocity;

                    //// Apply
                    //physic.globalVelocity += jumpVelocity;

                    // Position (p) :
                    //
                    //            at²
                    // p += vt + -----
                    //             2
                    //
                    // Change the global velocity sytem to a translation system
                    Vector3 translation = gravityJumpFallback.velocity * dt + 0.5f * gravityJumpFallback.acceleration * dt * dt;
                    physic.translate += translation;

                }
            }

        }

        /**************
         * CONTROLLER *
         **************/

        /// <summary>
        /// Tick jump related controller. Used for input related logic.
        /// </summary>
        /// <param name="input">The kid player's controller. Use it to retrieve his inputs.</param>
        /// <param name="dt">Time elapsed since last tick</param>
        private void JumpController(Controller input, float dt)
        {

            // If the player press the jump button input,
            if (input.kid.jump.WasPressed)
            {

                // Find witch jump to perfom
                Jump jumpToPerform = null;

                // If climbing,
                if (climbing.IsClimbing())
                {
                    jumpToPerform = wallJump;
                }
                else

                // If is curently perfoming any other jump than an air jump,
                if (Jump_IsJumping() && activeJump != airJump)
                {
                    // Then do an air jump
                    jumpToPerform = airJump;
                }
                else

                // If the player is grounded,
                if (IsGrounded())
                {
                    // Then do a regular jump.
                    jumpToPerform = regularJump;
                }

                // If a jump to perfom was found,
                if (jumpToPerform != null)
                {
                    // Perform this jump.

                    // End active jumpt before starting a new one.
                    Jump_EndActiveJump();

                    // Start the new jump.
                    jumpToPerform.StartJump();

                    // Aditional configuration for wall jumps.
                    bool isAWallJump = typeof(WallJump).IsAssignableFrom(jumpToPerform.GetType());
                    if (isAWallJump)
                    {
                        WallJump wallJump = (WallJump)jumpToPerform;
                        wallJump.AddWallImpule(characterCompass.transform.up);

                        // Then do a wall jump
                        climbing.ForceClimbingModeExit();
                    }

                    // Set this jump as the currently active jump.
                    activeJump = jumpToPerform;
                }

            }

            // If the player release the jump button input,
            if (input.kid.jump.WasReleased)
            {
                // Then notifie the active jump that the jump button was released.
                if (Jump_IsJumping())
                    activeJump.RefreshGravityAcceleration(false);
            }
        }



        /********
         * JUMP *
         ********/

        /// <summary>
        /// Class containing everything necesary to make the kid jump and configure the parabolic of this jump.
        /// </summary>
        [System.Serializable]
        public class Jump
        {

            // Configuration

            /// <summary>
            /// Container of jump configuration related data.
            /// </summary>
            [System.Serializable]
            public class JumpConfig
            {
                // Timing

                /// <summary>
                /// How much time does it takes for the player to reach the maximum jump height.
                /// </summary>
                [Header("Timing")]
                [Tooltip("How much time does it takes for the player to reach the maximum jump height.")]
                public float ascensionDuration = 0.25f;

                /// <summary>
                /// How much time does it takes for the player to fall from the maximum jump height to it's original position.
                /// </summary>
                [Tooltip("How much time does it takes for the player to fall from the maximum jump height to it's original position.")]
                public float fallDuration = 0.25f;


                // Height

                /// <summary>
                /// How high the player can jump.
                /// </summary>
                [Header("Height")]
                [Tooltip("How high the player can jump.")]
                public float maxJumpHeight = 3.0f;

                /// <summary>
                /// How much the gravity is increased when the player release the jump buton durring ascension.
                /// </summary>
                [Tooltip("How much the gravity is increased when the player release the jump buton durring ascension.")]
                public float releaseButtonGravityMultiplier = 2f;
            }

            /// <summary>
            /// Jump configuration related data.
            /// </summary>
            [SerializeField]
            [Tooltip("Jump configuration related data.")]
            public JumpConfig jumpConfig = new JumpConfig();



            // Physic
            /// <summary>
            /// The current jump velocity.
            /// </summary>
            [System.NonSerialized]
            public Vector3 velocity = Vector3.zero;

            /// <summary>
            /// The current gravity acceleration of this jump.
            /// </summary>
            [System.NonSerialized]
            public Vector3 acceleration = Vector3.zero;

            // State
            private bool hasReachedPeak = false;

            virtual protected float GetInitialVelocity(JumpConfig config)
            {

                // Jump parabola
                //
                //         ^
                //         |
                //      h -+              ...'''''...
                //         |        ...'''           '''...
                //         |     .''                       ''.
                //         |   .'                             '.
                //         |  '                                 '
                //         | '                                   '
                //         |'                                     '
                //         +-------------------+-------------------->
                //               assention     |        fall
                //                             th

                // Height (h) :
                float h = config.maxJumpHeight;

                // Duration to peak (th) :
                float th = config.ascensionDuration;

                // Initial Velocity (v0) :
                //       2h
                // v0 = ----
                //       th
                return (2 * h) / (th);
            }

            private float GetGravityAcceleration(JumpConfig config, bool isPressingJumpButton)
            {

                // Jump parabola
                //
                //         ^
                //         |
                //      h -+              ...'''''...
                //         |        ...'''           '''...
                //         |     .''                       ''.
                //         |   .'                             '.
                //         |  '                                 '
                //         | '                                   '
                //         |'                                     '
                //         +-------------------+-------------------->
                //               assention     |        fall
                //                             th

                // Height (h) :
                float h = config.maxJumpHeight;

                // Duration to peak (th) :
                float th = //config.ascensionDuration / 2;
                    !hasReachedPeak ?
                        config.ascensionDuration
                        :
                        // Is falling : Use max height
                        config.fallDuration
                    ;

                // Gravity (g) :
                //      -2 * h
                // g = --------
                //        th²
                float acceleration = (-2 * h) / (th * th);

                // Multiply gravity if the button is relesed
                acceleration *= isPressingJumpButton ? 1 : config.releaseButtonGravityMultiplier;

                return acceleration;
            }

            // 
            public virtual void StartJump()
            {
                hasReachedPeak = false;
                velocity = Vector3.up * GetInitialVelocity(jumpConfig);
                acceleration = Vector3.up * GetGravityAcceleration(jumpConfig, true);
            }

            public void EndJump()
            {
                velocity = Vector3.zero;
                hasReachedPeak = true;
            }

            // Tick
            public virtual void Tick(float dt)
            {
                // Velocity (v) :
                //
                // v += at
                //
                velocity += acceleration * dt;

                // Cheack if has reached peak
                if (Vector3.Dot(Vector3.up, velocity) < 0)
                {
                    if (hasReachedPeak == false)
                    {
                        hasReachedPeak = true;
                        RefreshGravityAcceleration();
                    }
                }

            }

            public void RefreshGravityAcceleration(bool isPressingJumpButton = false)
            {

                // Acceleration (v) :
                acceleration = Vector3.up * GetGravityAcceleration(jumpConfig, isPressingJumpButton);
            }

            public bool HasReachedPeak()
            {
                return hasReachedPeak;
            }

        }

        [System.Serializable]
        public class WallJump : Jump
        {

            [System.Serializable]
            public class WallJumpConfig
            {
                public float wallImpuleStrengh = 1;
                public float wallImpuleDuration = 1;
            }

            public WallJumpConfig wallJumpConfig = new WallJumpConfig();

            public void AddWallImpule(Vector3 wallNormal)
            {

                // Apply horizontal impule from wall normal
                // Ignore if,
                if (
                    wallJumpConfig.wallImpuleStrengh <= 0 ||    // The impulse is null
                    wallJumpConfig.wallImpuleDuration <= 0      // The duration is null
                    )
                    return;

                // Add directional (horizontal) impulse depending on the normal of the wall the player is jumping from.
                velocity += Vector3.ProjectOnPlane(wallJumpConfig.wallImpuleStrengh * wallNormal, Vector3.up);

            }

            public override void Tick(float dt)
            {
                base.Tick(dt);

                // Reset horizontal impule from wall normal
                // Ignore if,
                if (
                    wallJumpConfig.wallImpuleStrengh <= 0 ||    // The impulse is null
                    wallJumpConfig.wallImpuleDuration <= 0      // The duration is null
                    )
                    return;
                velocity = Vector3.MoveTowards(
                    velocity,
                    Vector3.Project(velocity, Vector3.up),
                    (wallJumpConfig.wallImpuleStrengh / wallJumpConfig.wallImpuleDuration) * dt
                    );
            }

        }

        // Jumps

        [Header("Jumps")]
        /// <summary>
        /// The most common jump the kid uses.
        /// </summary>
        [SerializeField]
        private Jump regularJump = new Jump();

        /// <summary>
        /// The jump the kid uses when he is in the air.
        /// </summary>
        [SerializeField]
        private Jump airJump = new Jump();

        /// <summary>
        /// The jump the kid uses when jumping from the climbing mode.
        /// </summary>
        [SerializeField]
        private WallJump wallJump = new WallJump();

        /// <summary>
        /// The jump the kid is currently using.
        /// </summary>
        [System.NonSerialized]
        public Jump activeJump = null;

        /// <summary>
        /// The jump whose gravity shall be used when the player isn't jumping. (Ex : fall from a cliff.)
        /// </summary>
        [System.NonSerialized]
        private Jump gravityJumpFallback = null;
    }

}