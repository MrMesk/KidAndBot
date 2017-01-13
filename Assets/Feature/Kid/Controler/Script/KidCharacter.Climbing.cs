﻿using UnityEngine;
using System.Collections;
using PlayerInput;
using System.Collections.Generic;

namespace Gameplay {

    public partial class KidCharacter : Character {

        /*********
         * LOGIC *
         *********/

        private void InitClimbing() {

        }

        private void ClimbingLogicTick(float dt) {

            // Climb logic
            if(!IsJumping() || activeJump.HasReachedPeak())
            {
                climbing.TickLogic(dt, transform);
            }

            // Position correction

            // Climbing indicator.
            if (climbing.IsClimbing()) {

                // Retrieve the collider the player is climbing
                Collider collider = climbing.attachedClimbableObject._collider;

                // Find the closest point from the player on the collider.
                var closestPointOnCollider = ColliderUtility.GetClosestPointOnClollider(collider, transform.position);

                // Find the closest point from the previously found point on the player
                var closestPointOnPlayer = CharacterControllerUtility.GetClosestPointOnCharacterController(controller, closestPointOnCollider.position);

                // Snap the player by move the point found on the player to the point found on the collider.
                Vector3 position = transform.position;
                position -= closestPointOnPlayer.position;
                position += closestPointOnCollider.position;
                //transform.position = position;

                Vector3 deltaPos = closestPointOnCollider.position - closestPointOnPlayer.position;
                physic.translate += deltaPos;

                try
                {
                    // Indicator A
                    var point = ColliderUtility.GetClosestPointOnClollider(collider, transform.position);
                    climbing.config.climbingIndicatorA.gameObject.SetActive(true);
                    climbing.config.climbingIndicatorA.position = point.position;
                    Debug.DrawRay(point.position, point.normal * 1.5f, Color.red);

                    // Indocator B
                    point = CharacterControllerUtility.GetClosestPointOnCharacterController(controller, point.position);
                    climbing.config.climbingIndicatorB.gameObject.SetActive(true);
                    climbing.config.climbingIndicatorB.position = point.position;
                    Debug.DrawRay(point.position, point.normal * 1.5f, Color.green);


                }
                catch (System.Exception e)
                {
                    Debug.LogError("Climbing a collider type not handled.\n" + e.TargetSite);
                }
            } else {
                //climbing.config.climbingIndicatorA.gameObject.SetActive(false);
                //climbing.config.climbingIndicatorB.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Return if the kid is curently climbing onto an object.
        /// </summary>
        /// <returns></returns>
        public override bool IsClimbing() {
            return climbing.attachedClimbableObject != null;
        }

        /**************
         * CONTROLLER *
         **************/

        private void ClimbingControllerTick(Controller input, float dt) {
            
        }

        /*****************
         * PHYSIC ENGINE *
         *****************/

        // Enter
        protected void OnCollisionEnterClimbing(Collision collision) {
            // Try to retrieve the climbable wall on the collided object to learn if it's climbable or not
            ClimbableWall climbableObject = collision.gameObject.GetComponent<ClimbableWall>();
            if (climbableObject != null) {
                // Connected with a climbable wall
                climbing.ConnectedWithClimbalbeObject(climbableObject);
                
                // TODO :
                // Need to refresh current parent
            }
        }

        // Exit
        protected void OnCollisionExitClimbing(Collision collision) {
            // Try to retrieve the climbable wall on the collided object to learn if it's climbable or not
            ClimbableWall climbableObject = collision.gameObject.GetComponent<ClimbableWall>();
            if (climbableObject != null) {
                // Disconected with a climbable wall
                climbing.DisconnectedWithClimbalbeObject(climbableObject);
               
                // TODO :
                // Need to refresh current parent
            }
        }

        /***********
         * CLIMING *
         ***********/

        /// <summary>
        /// Class containing everything necesary to make the kid jump and configure the parabolic of this jump.
        /// </summary>
        [System.Serializable]
        public class Climbing {

            // Configuration

            /// <summary>
            /// Container of jump configuration related data.
            /// </summary>
            [System.Serializable]
            public class Config {
                public Transform climbingIndicatorA;
                public Transform climbingIndicatorB;
            }

            /// <summary>
            /// Jump configuration related data.
            /// </summary>
            [SerializeField]
            public Config config = new Config();

            [System.NonSerialized]
            public List<ClimbableWall> connectedClimbableObjectList = new List<ClimbableWall>();

            [System.NonSerialized]
            public ClimbableWall attachedClimbableObject = null;

            // Flag informing if it's necesary to refresh the climbable object this character is attached to
            [System.NonSerialized]
            public bool needRefreshAttachedClimbableObject = false;

            // Events
            
            /// <summary>
            /// Eventcalled wenever the player change the climbalbe object he's attached to.
            /// </summary>
            public System.Action eAttachedToANewClimbalbeObject;

            /// <summary>
            /// Refresh the climbable object this character is attached to.
            /// </summary>
            public void RefreshAttachedClimbableObject(Transform characterTransform) {

                // Find the best climbable object the player should be attached to
                ClimbableWall bestClimbableObject = null;
                
                int count = connectedClimbableObjectList.Count;
                // If the player isn't connected to any climbable object
                if(count <= 0) {
                    // Then there isn't any best climbable object to be attached to
                    bestClimbableObject = null;
                }
                else

                // If the player is attached to only one climbable object
                if (count == 1) {
                    // Then there there is only one climbable object that is the best to be attached to
                    bestClimbableObject = connectedClimbableObjectList[0];
                }
                else

                // If the player is attached to many climbable objects
                if (count > 1) {
                    // Then the best climbalbe object is the closest

                    // Setup loop
                    ClimbableWall closestClimbableObject;
                    float distanceFromClosestClimbalbeObject;
                    float distanceFromCurrentClimbableObjectInLoop;

                    // Check first
                    bestClimbableObject = closestClimbableObject = connectedClimbableObjectList[0];
                    distanceFromClosestClimbalbeObject = Vector3.Distance(characterTransform.position, closestClimbableObject.transform.position);
                    
                    // Check next
                    for (int i = 1; i < count; ++i) {

                        closestClimbableObject = connectedClimbableObjectList[i];
                        distanceFromCurrentClimbableObjectInLoop = Vector3.Distance(characterTransform.position, closestClimbableObject.transform.position);

                        // If the currently checked climbable object is closer from the player than the prevously found climbalbe object,
                        if (distanceFromCurrentClimbableObjectInLoop < distanceFromClosestClimbalbeObject) {
                            // Then save it as the new closest one
                            distanceFromClosestClimbalbeObject = distanceFromCurrentClimbableObjectInLoop;
                        }
                    }

                    bestClimbableObject = closestClimbableObject;
                }

                // Attach the player to the best climbable object found.
                AttachToClimbableObject(bestClimbableObject);

                // Notify that the refresh was executed and isn't necesary anymore.
                needRefreshAttachedClimbableObject = false;
            }

            public void TickLogic(float dt, Transform characterTransform) {

                // Refresh the climbable object this character is attached to, if necesary.
                if (needRefreshAttachedClimbableObject) {
                    RefreshAttachedClimbableObject(characterTransform);
                }

            }

            public void ConnectedWithClimbalbeObject(ClimbableWall climbableObject) {
                // Add the newly connected climbable object to the connected climbable object list.
                connectedClimbableObjectList.Add(climbableObject);
                // Remember to refresh the climbable object this character is attached to at next logic tick.
                needRefreshAttachedClimbableObject = true;
            }

            public void DisconnectedWithClimbalbeObject(ClimbableWall climbableObject) {
                // Remove the disconnected climbable object from the connected climbable object list.
                connectedClimbableObjectList.Remove(climbableObject);
                // Remember to refresh the climbable object this character is attached to at next logic tick.
                needRefreshAttachedClimbableObject = true;
            }
            
            public void AttachToClimbableObject(ClimbableWall climbableObject) {
                // Exit if already attached to this climbable object
                if (attachedClimbableObject == climbableObject) return;

                // Detach from the climbable object the player was previously attached to, if there were one.
                if (attachedClimbableObject != null) {
                    // Notify the climbable object that the player just detached himself from it.
                    attachedClimbableObject.CleanHitMat();
                }

                // Attach to the new climbable object
                attachedClimbableObject = climbableObject;

                // Exit if the character just attached himself to nothing
                if (attachedClimbableObject == null) return;

                // Notify the climbable object that the player just attached himself to it.
                attachedClimbableObject.SetHitMat();

                // TODO :
                // Fake ground hit
                //Physic_ResetGravity();
                //if (physic.eHitGround != null) {
                //    physic.eHitGround.Invoke();
                //}

                // Notify that the player just attached himself to a new climbable object.
                if (eAttachedToANewClimbalbeObject != null)
                    eAttachedToANewClimbalbeObject();
            }

            public void ForceClimbingModeExit() {
                AttachToClimbableObject(null);
                needRefreshAttachedClimbableObject = false;
            }

            public bool IsClimbing() {
                return attachedClimbableObject != null;
            }

        }

        [Header("Climbing")]
        [SerializeField]
        public Climbing climbing = new Climbing();


    }

}