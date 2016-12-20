using UnityEngine;
using System;
using System.Collections;

public class CharacterCompass : MonoBehaviour {

    /// <summary>
    /// The character this compas is attached to.
    /// </summary>
    public Character character;
    
<<<<<<< HEAD
    public Vector3 localPosition = new Vector3(0, -1, 0);

	public Transform overrideCameraTransform;
    
    [NonSerialized] public Vector3 climbNormal;
=======
    /// <summary>
    /// The position of this compass offseted from it's attached character's position.
    /// </summary>
    public Vector3 positionOffset = new Vector3(0, -1, 0);
>>>>>>> develop

    public void Start() {
        // Store rotation
        transform.rotation = GetWalkingCompassRotation();
    }

    private void FixedUpdate() {
        // Rotation
        Quaternion targetRotation;
        try {
            // Try to get climbing compass rotation
            targetRotation = GetClimbingCompassRotation();
        } catch {
            // Get walking compas rotation if it can't
            targetRotation = GetWalkingCompassRotation();
            //targetRotation = Snap(targetRotation);
        }

        // Rotation smoothing
        float deltaAngle = Quaternion.Angle(transform.rotation, targetRotation);
        if(deltaAngle < 135) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, (deltaAngle * 4 + 180) * Time.deltaTime);
        } else {
            transform.rotation = targetRotation;
        }

        // Position
        transform.position = character.transform.position + positionOffset;
    }

    public Quaternion GetWalkingCompassRotation() {
        // Get camera forward
		Vector3 cameraForward = overrideCameraTransform != null ? overrideCameraTransform.forward : character.characterCamera.transform.forward;
        // Project on ground
        cameraForward = Vector3.ProjectOnPlane(cameraForward, character.physic.gravity);
        // Convert to quaternion/rotation
        Quaternion forwardRotation = Quaternion.LookRotation(cameraForward, -character.physic.gravity);
        return forwardRotation;
    }

    public Quaternion GetClimbingCompassRotation() {
        // Get kid character
        if(character.GetType() != typeof(KidCharacter)) {
            throw new System.Exception();
        }
        KidCharacter kidCharacter = (KidCharacter)character;

        // Get kid's position
        Vector3 kidPosition = kidCharacter.transform.position;

        // Get kid's climbing collider
        Collider currentlyClimbingCollider = kidCharacter._selectedClimbableWall._collider;
                
        // Get closest point on climbing collider
        var pointData = ColliderHelper.GetClosestPointOnClollider(currentlyClimbingCollider, kidPosition);
        // Get this point's normal
        Vector3 normal = pointData.normal;        

        // Camera
		Transform kidCamera = overrideCameraTransform != null ? overrideCameraTransform : kidCharacter.characterCamera.transform;

        // Rotation upward
        Vector3 upward = Vector3.ProjectOnPlane(normal, -kidCamera.right);
        // Rotation forward
        Vector3 forward = Quaternion.AngleAxis(90, kidCamera.right) * upward;
        // Rotation
        Quaternion rotation = Quaternion.LookRotation(forward, upward);

        // Balance
        float balanceDot = Vector3.Dot(normal, -kidCamera.right);
        // Balance application
        rotation = Quaternion.AngleAxis(90 * balanceDot, forward) * rotation;

        return rotation;
    }

    [Obsolete]
    public Quaternion Snap(Quaternion rotation) {
        const float snapAngle = 45;
        Vector3 eulerAngle = rotation.eulerAngles;
        eulerAngle = new Vector3(
            Mathf.Round(eulerAngle.x / snapAngle) * snapAngle,
            Mathf.Round(eulerAngle.y / snapAngle) * snapAngle,
            Mathf.Round(eulerAngle.z / snapAngle) * snapAngle
            );
        return Quaternion.Euler(eulerAngle);
    }

}
