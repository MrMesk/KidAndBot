using UnityEngine;
using System.Collections;

namespace Gameplay {

    public class KidCompass : CharacterCompass {

        public KidCharacter kidCharacter;

        [System.NonSerialized]
        private Vector3 _lastNormal;
        [System.NonSerialized]
        public Vector3 normal;

        public System.Action eDrasticlyChangedNormal;

        protected override void FixedUpdate() {

            // The most appropriate rotation the compass should take to match the player needs.
            Quaternion targetRotation = Quaternion.identity;

            if (kidCharacter.IsClimbing()) {
                targetRotation = GetClimbingCompassRotation();
            } else {
                targetRotation = GetWalkingCompassRotation();
                _lastNormal = normal;
                normal = Vector3.up;
            }

            if(_lastNormal.y > normal.y)
            {
                if (eDrasticlyChangedNormal != null)
                    eDrasticlyChangedNormal();
            }

            // Rotation smoothing
            float deltaAngle = Quaternion.Angle(transform.rotation, targetRotation);
            if (deltaAngle < 135) {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, (deltaAngle * 4 + 180) * Time.deltaTime);
            } else {
                transform.rotation = targetRotation;
                if (eDrasticlyChangedNormal != null)
                    eDrasticlyChangedNormal();
            }

            // Position (Only visual, not usefull)
            transform.position = character.transform.position + positionOffset;
        }

        public override Quaternion GetClimbingCompassRotation() {
            
            // Get kid's position
            Vector3 kidPosition = kidCharacter.transform.position;

            // Get the climbable object the kid is climbing on.
            ClimbableWall attachedClimbalbeOject = kidCharacter.climbing.attachedClimbableObject;

            // Get the collider of this climbable object
            Collider acoCollider = attachedClimbalbeOject._collider;


            // TODO : Improve the folowing
            // Get closest point from the kid's center of mass on the climbable object's collider.
            var pointData = ColliderUtility.GetClosestPointOnClollider(acoCollider, kidPosition);
            // Get this point's normal.
            _lastNormal = this.normal;
            Vector3 normal = this.normal = pointData.normal;

            // Retrieve the transform of the camera attached to the kid
            Transform cameraTransform = overrideCameraTransform != null ? overrideCameraTransform : kidCharacter.characterCamera.transform;
                        
            if (acoCollider.GetType() == typeof(BoxCollider))
            {
                // Compute the best rotation from the point found's normal.
                // Rotation upward
                Vector3 upward = normal;

                float dot = Vector3.Dot(Vector3.up, Vector3.Project(normal.normalized, Vector3.up));
                dot = Mathf.Asin(dot) / (Mathf.PI/2);

                Debug.DrawRay(transform.position, normal, Color.Lerp(Color.magenta, Color.Lerp(Color.cyan, Color.yellow, dot), dot + 1));

                if(dot < 0.75f && dot > -0.75f)
                {
                    // Rotation forward
                    Vector3 right = Quaternion.AngleAxis(-90, Vector3.up) * Vector3.ProjectOnPlane(normal, Vector3.up).normalized;
                    Debug.DrawRay(transform.position, right * 2);

                    Vector3 forward = Quaternion.AngleAxis(90, right) * upward;
                    // Found rotation
                    Quaternion rotation = Quaternion.LookRotation(forward, upward);

                    // Return the found rotation.
                    return rotation;
                }
                else
                {
                    // Rotation forward
                    Vector3 forward = Quaternion.AngleAxis(90, cameraTransform.right) * upward;
                    // Found rotation
                    Quaternion rotation = Quaternion.LookRotation(forward, upward);

                    // Return the found rotation.
                    return rotation;
                }

            }
            else if(acoCollider.GetType() == typeof(SphereCollider))
            {
                // Compute the best rotation from the point found's normal.
                // Rotation upward
                Vector3 upward = Vector3.ProjectOnPlane(normal, -cameraTransform.right);
                // Rotation forward
                Vector3 forward = Quaternion.AngleAxis(90, cameraTransform.right) * upward;
                // Found rotation
                Quaternion rotation = Quaternion.LookRotation(forward, upward);

                // Balance this rotation left or right depending of the slope
                // Balance
                float balanceDot = Vector3.Dot(normal, -cameraTransform.right);
                balanceDot = Mathf.Asin(balanceDot) / (Mathf.PI / 2);
                // Balance applications
                rotation = Quaternion.AngleAxis(90 * balanceDot, forward) * rotation;

                // Return the found rotation.
                return rotation;
            }

            throw new System.NotImplementedException();

        }

    }

}