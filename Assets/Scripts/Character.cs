using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour {

    [Header("Character Config")]
    [Space(8)]

    public CharacterCamera characterCamera;
    public CharacterCompass characterCompass;
    public Transform abilityContainer;

    // Look
    [NonSerialized] public Quaternion lookRotation;

    // Horizontal mobility
    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    public Vector3 gravityVelocity = Vector3.zero;

    // Physic
    [NonSerialized] public Vector3 globalVelocity = Vector3.zero;
    [NonSerialized] public BasicEvent eHitGround;

    // Mobility
    public enum MobilityState {
        GROUNDED,
        AIRBORN,
        JUMPING,
        CLIMBING,
        LEAPING
    }
    [NonSerialized]
    public MobilityState mobilityState = MobilityState.AIRBORN;
    /* grounded
     * airborn
     * climbing
     */

    // State
    [NonSerialized] public CharacterController controller;
    [NonSerialized] protected bool isClimbing = false;
    [NonSerialized] protected Vector2 _directionalInput;

    protected virtual void Awake() {
        // Retrieve component(s)
        controller = GetComponent<CharacterController>();
    }

	protected virtual void Update() { }

    protected virtual void LateUpdate() {
        // Refresh look direction
        transform.rotation = lookRotation;
    }

    protected virtual void FixedUpdate() {
        //DefaultController();
        ApplyGravity();
        ComputeCollisions();
    }
    
    public void ResetGravity() {
        gravityVelocity = Vector3.zero;
    }

    protected void ApplyGravity() {
        // Do not apply gravity if grounded
        if (IsGrounded()) return;
        // Do not apply gravity if climbing
        if (IsClimbing()) return;

        // Refresh gravity velocity by applying gravity acceleration
        gravityVelocity += gravity * Time.fixedDeltaTime;

        // Refresh global velocity
        globalVelocity += gravityVelocity;        
    }

    protected virtual void ComputeCollisions() {
        // Execute movement
        CollisionFlags collisionFlags = controller.Move(globalVelocity * Time.fixedDeltaTime);
        
        // Check if collided with the ground
        if (
            ((collisionFlags & CollisionFlags.Below) != 0) &&
            globalVelocity.y < 0
        ) {
            // Reset gravity
            ResetGravity();
            // Notifie collision with ground
            if (eHitGround != null) {
                eHitGround.Invoke();
            }
        }

        // Clean global velocity
        globalVelocity = Vector3.zero;
    }

    public virtual bool IsGrounded() {
        return (controller.collisionFlags & CollisionFlags.Below) != 0;
    }
    
    // Ability state
    public virtual bool IsClimbing() {
        return false;
    }
    public virtual bool IsGrabbing() {
        return false;
    }
}
