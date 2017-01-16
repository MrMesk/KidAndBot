using UnityEngine;
using System.Collections;
using PlayerInput;
using System.Collections.Generic;

namespace Gameplay {

    public partial class KidCharacter : Character {

        /*********
         * LOGIC *
         *********/
        
        public void Adherence_LogicTick(float dt)
        {
            if (adherence.needAdherenceRefresh)
            {
                Adherence_RefreshCurrentParent();
            }
        }

        public void Adherence_OnCollisionEnter(Collision collision)
        {
            // Connected to collider
            adherence.ConnectToCollider(collision.collider);
        }

        public void Adherence_OnCollisionExit(Collision collision)
        {
            // Disconnect from collider
            adherence.DisconectFromCollider(collision.collider);
        }

        public void Adherence_RefreshCurrentParent()
        {

            adherence.needAdherenceRefresh = false;

            Transform targetParent = null;

            // If climbing,
            if (climbing.IsClimbing())
            {
                // Then select climbable's parent
                targetParent = Adherence.FindFirstUnscaledParent(climbing.attachedClimbableObject.transform);

            }
            else
            {
                // Otherwise, select the best parent from the adherence system
                targetParent = adherence.FindBestParent(this, directional.velocity);
            }
            
            // Adhere to nothing if there is no parent to attach to
            if (targetParent == null)
            {
                transform.SetParent(null);
				transform.localScale = Vector3.one;
                return;
            }
            
            // Adjhre to the target parent
            if (targetParent != transform.parent)
            {
				transform.SetParent(targetParent);
				Vector3 scaleCorrection = targetParent.lossyScale;
				scaleCorrection = new Vector3 (1 / scaleCorrection.x, 1 / scaleCorrection.y, 1 / scaleCorrection.z);
				transform.localScale = scaleCorrection;
            }
        }

        /**************
         * CONTROLLER *
         **************/



        /*************
         * ADHERENCE *
         *************/

        [System.Serializable]
        public class Adherence {

            // Configuration
            
            [System.Serializable]
            public class AdherenceConfig
            {

            }
            
            [SerializeField]
            public AdherenceConfig config = new AdherenceConfig();

            [System.NonSerialized]
            private List<Collider> connectedColliders = new List<Collider>();

            [System.NonSerialized]
            public bool needAdherenceRefresh = false;

            public void ConnectToCollider(Collider collider)
            {
                if (!connectedColliders.Contains(collider))
                {
                    connectedColliders.Add(collider);
                    needAdherenceRefresh = true;
                }
            }

            public void DisconectFromCollider(Collider collider)
            {
                if (connectedColliders.Contains(collider))
                {
                    connectedColliders.Remove(collider);
                    needAdherenceRefresh = true;
                }
            }
            
            public Transform FindBestParent(Character character, Vector3 intentionedDeplacement)
            {

                Transform bestParent = null;

                if (connectedColliders.Count <= 0)
                {
                    bestParent = null;
                }
                else if (connectedColliders.Count == 1)
                {
                    // Select connected colliders's parent
                    bestParent = FindFirstUnscaledParent(connectedColliders[0].transform);
                }
                else
                {
                    // Select closest connected colliders's parent

                    // Get closest collider

                    // Setup loop
                    // Closest collider data
                    Collider closestCollider;
                    float distToClosest;
                    // Current collider data
                    Collider currentCollider = connectedColliders[0];
                    float distToCurrentCollider = GetDistanceBetween(character.transform, currentCollider, intentionedDeplacement);
                    // Select first collider
                    closestCollider = currentCollider;
                    distToClosest = distToCurrentCollider;
                    // Check for remaining colliders
                    for (int i = 1; i < connectedColliders.Count; ++i)
                    {
                        // Select collider
                        currentCollider = connectedColliders[i];
                        distToCurrentCollider = GetDistanceBetween(character.transform, currentCollider, intentionedDeplacement);
                        // Check if it's a better match
                        if (distToCurrentCollider < distToClosest)
                        {
                            // Select it if it is
                            closestCollider = currentCollider;
                            distToClosest = distToCurrentCollider;
                        }

                    }

                    // Get closest collider parent
                    bestParent = FindFirstUnscaledParent(closestCollider.transform);
                }

                return bestParent;
            }

            // Utility

            public static Transform FindFirstUnscaledParent(Transform transform)
            {
				return transform;

                Transform parent = transform.parent;
                while (parent != null)
                {
                    if (parent.lossyScale == Vector3.one)
                    {
                        break;
                    }
                    parent = parent.parent;
                }
                return parent;
            }

            public static float GetDistanceBetween(Transform characterTransform, Collider collider, Vector3 intentionedDeplacement)
            {
                try
                {
                    const float intentionedMultiplicator = 1f;
                    Vector3 intentionedPosition = Vector3.MoveTowards(
                        characterTransform.position,
                        collider.transform.position,
                        Vector3.Dot(
                            intentionedDeplacement * intentionedMultiplicator,
                            collider.transform.position - characterTransform.transform.position
                            )
                    );
                    var closestPointOnCollider =
                        ColliderUtility.GetClosestPointOnClollider(
                            collider,
                            intentionedPosition
                            );
                    return Vector3.Distance(
                        closestPointOnCollider.position,
                        intentionedPosition
                        );
                }
                catch
                {
                    return Vector3.Distance(
                        characterTransform.transform.position,
                        collider.ClosestPointOnBounds(characterTransform.transform.position)
                        );
                }
            }

        }

        [Header("Directional")]
        [SerializeField]
        public Adherence adherence = new Adherence();

    }

}