using UnityEngine;
using System.Collections;

public class CharacterCompass : MonoBehaviour {

    public Character _attachedCharacter;

    public Transform correctedCompas;

    public Vector3 localPosition = new Vector3(0, -1, 0);

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

        float deltaAngle = Quaternion.Angle(transform.rotation, targetRotation);
        if(deltaAngle < 135) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, (deltaAngle * 4 + 180) * Time.deltaTime);
        } else {
            transform.rotation = targetRotation;
        }
        correctedCompas.rotation = targetRotation;

        // Position
        transform.position = _attachedCharacter.transform.position + localPosition;
    }

    public Quaternion GetWalkingCompasRotation() {
        // Get camera forward
        Vector3 cameraForward = _attachedCharacter.characterCamera.transform.forward;
        // Project on ground
        cameraForward = Vector3.ProjectOnPlane(cameraForward, _attachedCharacter.gravity);
        // Convert to quaternion/rotation
        Quaternion forwardRotation = Quaternion.LookRotation(cameraForward, -_attachedCharacter.gravity);
        return forwardRotation;
    }

    public Quaternion GetClimbingCompasRotation() {
        // Check if attached character is a kid
        if (_attachedCharacter.GetType() != typeof(KidCharacter))
            throw new System.Exception();
        KidCharacter attachedKidCharacter = (KidCharacter)_attachedCharacter;
        // Check if attached character is climbing
        ClimbableWall wall = attachedKidCharacter._selectedClimbableWall;
        if (wall == null)
            throw new System.Exception();
        // Get camera forward
        Vector3 forward = Vector3.ProjectOnPlane(wall.transform.forward, Vector3.up);
        Vector3 right = Vector3.ProjectOnPlane(wall.transform.right, Vector3.up);
        Vector3 cameraOnRight = Vector3.Project(Vector3.ProjectOnPlane(_attachedCharacter.characterCamera.transform.forward, Vector3.up).normalized, right);
        Vector3 upwards = -Vector3.Project(_attachedCharacter.characterCamera.transform.forward, forward);
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
        // Get kid
        if(_attachedCharacter.GetType() != typeof(KidCharacter)) {
            throw new System.Exception();
        }
        KidCharacter kidCharacter = (KidCharacter)_attachedCharacter;


        // Get kid's climbing collider
        Collider currentlyClimbingCollider = kidCharacter._selectedClimbableWall._collider;

        // Get kid's position
        Vector3 kidPosition = kidCharacter.transform.position;

        // Get kid's foot
        Vector3 kidFootPosition = kidPosition + (-_attachedCharacter.transform.up * (_attachedCharacter.transform.localScale.y / 2));

        var pointData = ColliderHelper.GetClosestPointOnClollider(currentlyClimbingCollider, kidPosition);
        
        Debug.DrawRay(pointData.position, pointData.normal, Color.green);
        
        Vector3 normal = pointData.normal;

        

        CharacterCamera kidCamera = kidCharacter.characterCamera;
        Vector3 cameraForward = Vector3.ProjectOnPlane(kidCamera.transform.forward, Vector3.up);
        //cameraForward = kidCamera.transform.forward;

        Vector3 upward = Vector3.ProjectOnPlane(normal, -kidCamera.transform.right);
        Vector3 forward = Quaternion.AngleAxis(90, kidCamera.transform.right) * upward;
        Quaternion rotation = Quaternion.LookRotation(forward, upward);
        float balanceDot = Vector3.Dot(normal, -kidCamera.transform.right);
        rotation = Quaternion.AngleAxis(90 * balanceDot, forward) * rotation;

        //Debug.DrawRay(kidCharacter.transform.position, kidCamera.transform.right, Color.green);
        //Debug.DrawRay(kidCharacter.transform.position, upward, Color.yellow);

        //Debug.Log(balanceDot);

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
