using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class KidCharacter : Character {

    //protected override void Awake() {
    //    base.Awake();

    //}

    protected override void Update() {
        base.Update();
    }

    protected override void FixedUpdate() {
        if (_needRefreshSelectedClimbableWall) {
            RefreshSelectedClimbableWall();
        }

        base.FixedUpdate();
    }

    //protected void ClimbingController(Climbable climbableObject) {
    //    // Directional
    //    Vector3 cameraForward = _characterCamera.transform.forward;
    //    cameraForward = Vector3.ProjectOnPlane(cameraForward, _gravity);
    //    Quaternion forwardRotation = Quaternion.LookRotation(cameraForward, -_gravity);
    //    Vector3 directionalInput = new Vector3(_directionalInput.x, 0, _directionalInput.y);
    //    directionalInput = forwardRotation * directionalInput;
    //    directionalInput *= _moveSpeed;

    //    // Global
    //    Vector3 globalVelocity = directionalInput;
    //    if (isClimbing) {
    //        globalVelocity = Vector3.zero;
    //    }

    //    CollisionFlags collisionFlags = controller.Move(globalVelocity * Time.fixedDeltaTime);

    //    // Refresh rotation
    //    if (_directionalInput.magnitude > 0.1 /* deadzone */) {
    //        Quaternion directionalRotation = Quaternion.LookRotation(directionalInput, -_gravity);
    //        transform.rotation = directionalRotation;
    //    }

    //    if ((collisionFlags & CollisionFlags.Sides) != 0) {
    //        // Touching sides
    //    }

    //    if ((collisionFlags & CollisionFlags.Above) != 0) {
    //        // Touching ceiling
    //    }
    //}

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

    [NonSerialized] public List<ClimbableWall> _triggeredClimbableWalls = new List<ClimbableWall>();
    [NonSerialized] public ClimbableWall _selectedClimbableWall = null;

    private bool _needRefreshSelectedClimbableWall = false;

    public void SelectNewClimbableWall(ClimbableWall newClimbableWall) {
        if (_selectedClimbableWall == newClimbableWall) return;
        if (_selectedClimbableWall != null) {
            // Unselect
            _selectedClimbableWall.CleanHitMat();
            // Fake ground hit
            ResetGravity();
            if (eHitGround != null) {
                eHitGround.Invoke();
            }

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
            RefreshCurrentParent();
        }
    }
    protected void OnTriggerExit(Collider other) {
        ClimbableWall cw = other.gameObject.GetComponent<ClimbableWall>();
        if (cw != null) {
            // Exited climbable wall trigger
            _triggeredClimbableWalls.Remove(cw);
            RefreshCurrentParent();
        }
    }

    protected void OnCollisionEnter(Collision collision) {
        if(collision.gameObject == gameObject) {
            return;
        }

        if (!connectedColliders.Contains(collision.collider)) {
            connectedColliders.Add(collision.collider);
            RefreshCurrentParent();
        }
    }

    protected void OnCollisionExit(Collision collision) {
        if (collision.gameObject == gameObject) {
            return;
        }

        if (connectedColliders.Contains(collision.collider)) {
            connectedColliders.Remove(collision.collider);
            RefreshCurrentParent();
        }
    }
    

    public override bool IsClimbing() {
        return _selectedClimbableWall != null;
    }
    
    [NonSerialized] public List<Collider> connectedColliders = new List<Collider>();

    public void RefreshCurrentParent() {

        Transform targetParent = null;
        if (_selectedClimbableWall != null) {

            // Select climbable's parent
            targetParent = FindClosestUnscaledParent(_selectedClimbableWall.transform);

        } else if (connectedColliders.Count == 1) {

            // Select connected colliders's parent
            targetParent = FindClosestUnscaledParent(connectedColliders[0].transform);

        } else if (connectedColliders.Count > 1) {

            // Select closest connected colliders's parent
            Collider closestCollider;
            float distToClosest;

            Collider currentCollider;
            float distToCurrentCollider;

            closestCollider = connectedColliders[0];
            distToClosest = Vector3.Distance(transform.position, closestCollider.ClosestPointOnBounds(transform.position));

            for (int i = 1; i < connectedColliders.Count; ++i) {

                currentCollider = connectedColliders[i];
                distToCurrentCollider = Vector3.Distance(transform.position, currentCollider.ClosestPointOnBounds(transform.position));

                if(distToCurrentCollider < distToClosest) {
                    closestCollider = currentCollider;
                    distToClosest = distToCurrentCollider;
                }

            }

            targetParent = FindClosestUnscaledParent(closestCollider.transform);
        }

        // If current parent is the same or if current parent is higher in hierarchy
        if(targetParent == null) {
            transform.SetParent(targetParent);
            return;
        }

        if (transform.parent == null) {
            transform.SetParent(targetParent);
            return;
        }

        if (targetParent == transform.parent) {
            return;
        }

        if (targetParent.IsChildOf(transform.parent)) {
            return;
        }

        transform.SetParent(targetParent);

    }

    public Transform FindClosestUnscaledParent(Transform child) {
        Transform parent = child.parent;
        while(parent != null) {
            if(parent.lossyScale == Vector3.one) {
                break;
            }
            parent = parent.parent;
        }
        return parent;
    }

}
