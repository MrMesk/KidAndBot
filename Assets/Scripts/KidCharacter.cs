using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KidCharacter : Character {

    protected override void Awake() {
        base.Awake();
        
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();

        if (_needRefreshSelectedClimbableWall) {
            RefreshSelectedClimbableWall();
        }
    }

    protected void ClimbingController(Climbable climbableObject) {
        // Directional
        Vector3 cameraForward = _characterCamera.transform.forward;
        cameraForward = Vector3.ProjectOnPlane(cameraForward, _gravity);
        Quaternion forwardRotation = Quaternion.LookRotation(cameraForward, -_gravity);
        Vector3 directionalInput = new Vector3(_directionalInput.x, 0, _directionalInput.y);
        directionalInput = forwardRotation * directionalInput;
        directionalInput *= _moveSpeed;

        // Global
        Vector3 globalVelocity = directionalInput;
        if (isClimbing) {
            globalVelocity = Vector3.zero;
        }

        CollisionFlags collisionFlags = _characterController.Move(globalVelocity * Time.fixedDeltaTime);

        // Refresh rotation
        if (_directionalInput.magnitude > 0.1 /* deadzone */) {
            Quaternion directionalRotation = Quaternion.LookRotation(directionalInput, -_gravity);
            transform.rotation = directionalRotation;
        }

        if ((collisionFlags & CollisionFlags.Sides) != 0) {
            // Touching sides
        }

        if ((collisionFlags & CollisionFlags.Above) != 0) {
            // Touching ceiling
        }
    }

    public void RefreshSelectedClimbableWall() {
        ClimbableWall selection = null;

        int count = _triggeredClimbableWalls.Count;
        if (count == 1) {
            selection = _triggeredClimbableWalls[0];
        } else if (count > 1) {
            // Check whose the closest
            // Setup loop
            ClimbableWall climbableWall;
            float closestDistance;
            float dist;
            // Check first
            selection = climbableWall = _triggeredClimbableWalls[0];
            closestDistance = Vector3.Distance(transform.position, climbableWall.transform.position);
            // Check next
            for (int i = 1; i < count; ++i) {
                climbableWall = _triggeredClimbableWalls[i];
                dist = Vector3.Distance(transform.position, climbableWall.transform.position);
                if (dist < closestDistance) {
                    closestDistance = dist;
                    selection = climbableWall;
                }
            }
        }

        SelectNewClimbableWall(selection);

        if (selection = null) _needRefreshSelectedClimbableWall = false;
    }
    
    public List<ClimbableWall> _triggeredClimbableWalls = new List<ClimbableWall>();
    public ClimbableWall _selectedClimbableWall = null;

    private bool _needRefreshSelectedClimbableWall = false;

    public void SelectNewClimbableWall(ClimbableWall newClimbableWall) {
        if (_selectedClimbableWall == newClimbableWall) return;
        if (_selectedClimbableWall != null) {
            // Unselect
            _selectedClimbableWall.CleanHitMat();
        }
        // Select
        _selectedClimbableWall = newClimbableWall;
        if (_selectedClimbableWall == null) return;
        _selectedClimbableWall.SetHitMat();
    }

    protected void OnTriggerEnter(Collider other) {
        ClimbableWall cw = other.gameObject.GetComponent<ClimbableWall>();
        if (cw != null) {
            // Entred climbable wall trigger
            _needRefreshSelectedClimbableWall = true;
            _triggeredClimbableWalls.Add(cw);
        }
    }
    protected void OnTriggerExit(Collider other) {
        ClimbableWall cw = other.gameObject.GetComponent<ClimbableWall>();
        if (cw != null) {
            // Exited climbable wall trigger
            _triggeredClimbableWalls.Remove(cw);
        }
    }

    protected override bool IsClimbing() {
        return _selectedClimbableWall != null;
    }
}
