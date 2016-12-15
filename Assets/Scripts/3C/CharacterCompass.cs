using UnityEngine;
using System;
using System.Collections;

public class CharacterCompass : MonoBehaviour {

    /// <summary>
    /// The character this compas is attached to
    /// </summary>
    public Character character;
    
    public Vector3 localPosition = new Vector3(0, -1, 0);

	public Transform overrideCameraTransform;
    
    [NonSerialized] public Vector3 climbNormal;

    public void Start() {
        // Store rotation
        transform.rotation = GetWalkingCompasRotation();
    }

    private void FixedUpdate() {
        // Rotation
        Quaternion targetRotation;
        try {
            targetRotation = GetClimbingCompasRotation2();
        } catch {
            targetRotation = GetWalkingCompasRotation();
            targetRotation = Snap(targetRotation);
        }
        // Smoothing
        float deltaAngle = Quaternion.Angle(transform.rotation, targetRotation);
        if(deltaAngle < 135) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, (deltaAngle * 4 + 180) * Time.deltaTime);
        } else {
            transform.rotation = targetRotation;
        }

        // Position
        transform.position = character.transform.position + localPosition;
    }

    public Quaternion GetWalkingCompasRotation() {
        // Get camera forward
		Vector3 cameraForward = overrideCameraTransform != null ? overrideCameraTransform.forward : character.characterCamera.transform.forward;
        // Project on ground
        cameraForward = Vector3.ProjectOnPlane(cameraForward, character.gravity);
        // Convert to quaternion/rotation
        Quaternion forwardRotation = Quaternion.LookRotation(cameraForward, -character.gravity);
        return forwardRotation;
    }

    [Obsolete]
    // Legacy
    public Quaternion GetClimbingCompasRotation() {
        // Check if attached character is a kid
        if (character.GetType() != typeof(KidCharacter))
            throw new System.Exception();
        KidCharacter attachedKidCharacter = (KidCharacter)character;
        // Check if attached character is climbing
        ClimbableWall wall = attachedKidCharacter._selectedClimbableWall;
        if (wall == null)
            throw new System.Exception();
        // Get camera forward
        Vector3 forward = Vector3.ProjectOnPlane(wall.transform.forward, Vector3.up);
        Vector3 right = Vector3.ProjectOnPlane(wall.transform.right, Vector3.up);
        Vector3 cameraOnRight = Vector3.Project(Vector3.ProjectOnPlane(character.characterCamera.transform.forward, Vector3.up).normalized, right);
        Vector3 upwards = -Vector3.Project(character.characterCamera.transform.forward, forward);
        float dotForward = Vector3.Dot(forward, upwards);
        float dotRight = Vector3.Dot(right, cameraOnRight);
        Vector3 up = (dotForward > 0) ? wall.transform.up : -wall.transform.up;
        Quaternion climbingRotation;

        if (dotRight < -0.80f) {
            upwards = -cameraOnRight;
            climbingRotation = Quaternion.LookRotation(up, upwards);
        } else if (dotRight > 0.80f) {
            upwards = -cameraOnRight;
            climbingRotation = Quaternion.LookRotation(up, upwards);
        } else {
            if (dotForward > 0) {
                climbingRotation = Quaternion.LookRotation(up, upwards);
            } else {
                climbingRotation = Quaternion.LookRotation(up, -upwards);
            }
        }

        return climbingRotation;
    }

    public Quaternion GetClimbingCompasRotation2() {
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
        Vector3 normal = climbNormal = pointData.normal;        

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
