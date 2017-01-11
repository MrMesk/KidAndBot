﻿using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour
{

    // References

    [Header("Character Config")]
    [Space(8)]
    /// <summary>
    /// The camera attached to this character.
    /// </summary>
    public CharacterCamera characterCamera;

    /// <summary>
    /// The controller compass attached to this character.
    /// </summary>
    public CharacterCompass characterCompass;

    /// <summary>
    /// The <Ability> container attached to this character
    /// </summary>
    [Obsolete]
    public Transform abilityContainer;

    /// <summary>
    /// Attached unity character controller
    /// </summary>
    [NonSerialized]
    public CharacterController controller;




    // Look
    /// <summary>
    /// The direction this character is facing.
    /// </summary>
    [NonSerialized]
    public Quaternion lookRotation;



    /*****************
     * PHYSIC ENGINE *
     *****************/

    /// <summary>
    /// Container of physic related members.
    /// </summary>
    public class Physic
    {
        /// <summary>
        /// The current gravity acceleration of this character.
        /// </summary>
        public Vector3 gravity = new Vector3(0, -9.81f, 0);

        /// <summary>
        /// The current gravity velocity of this character.
        /// </summary>
        [NonSerialized]
        public Vector3 gravityVelocity = Vector3.zero;

        /// <summary>
        /// The global velocity of this character, applied and reset every frames.
        /// To apply a velocity onto this character, use the += statement each frame.
        /// </summary>
        [NonSerialized]
        public Vector3 globalVelocity = Vector3.zero;

        /// <summary>
        /// The translate vector is applied and reset every frames.
        /// To apply a translation onto this character, use the += statement each frame.
        /// </summary>
        [NonSerialized]
        public Vector3 translate = Vector3.zero;

        /// <summary>
        /// Event called wenever this character touches the ground.
        /// </summary>
        [NonSerialized]
        public Action eHitGround;

        /// <summary>
        /// Flag idicating if the character's foot where touching the ground last FixedUpdate().
        /// </summary>
        [NonSerialized]
        public bool touchedGroundLastFixedUpdate = false;
    }

    /// <summary>
    /// Container of physic related members.
    /// </summary>
    public Physic physic = new Physic();




    // Input

    /// <summary>
    /// Get controller handler
    /// </summary>
    /// <returns></returns>
    protected delegate PlayerInput.Controller GetControllerHandler();

    /// <summary>
    /// Get controller delegate
    /// </summary>
    protected GetControllerHandler getController;

    /// <summary>
    /// Attached input system
    /// </summary>
    public PlayerInput.Controller input { get { return getController(); } }




    // Unity

    /// <summary>
    /// Awake is called after this class's constructor.
    /// </summary>
    protected virtual void Awake()
    {
        // Retrieve component(s)
        // Retrieve the attahced unity character controller.
        controller = GetComponent<CharacterController>();

        // Retrieve input controller
        var characterType = this.GetType();
        if (
            characterType == typeof(KidCharacter) ||
            characterType == typeof(Gameplay.KidCharacter)
            )
        {
            getController = delegate ()
            {
                return PlayerInput.Controller.kidController;
            };
        }
        else if (characterType == typeof(RobotCharacter))
        {
            getController = delegate ()
            {
                return PlayerInput.Controller.botController;
            };
        }
    }

    /// <summary>
    /// Controller tick is called every frame. Use it for everything input related.
    /// </summary>
	protected virtual void Update()
    {
        // Execute 
        ControllerTick(input, Time.deltaTime);
    }

    /// <summary>
    /// Late update is called at the end of every frame. Use it for everything rendering related.
    /// </summary>
    protected virtual void LateUpdate()
    {
        // Refresh look direction
        transform.rotation = lookRotation;
        // Apply rendering
        Rendering(Time.deltaTime);
    }

    /// <summary>
    /// Update is called more than once a frame. Use it for everything logic related.
    /// </summary>
    protected virtual void FixedUpdate()
    {
        // Apply logic
        LogicTick(Time.fixedDeltaTime);
        // Calculate curent gravity velocity and apply it onto the global velocity
        //Physic_ApplyGravityOntoGlobalVelocity();
        // Apply the global velocity onto this character
        CollisionFlags collisionFlags =
            Physic_ApplyTranslationAndGlobalVelocity();
        // Compute collisions generated by the physical engine
        Physic_ComputeCollisionFlags(collisionFlags);
        // Reset the global velocity
        Physic_ResetTranslationAndGlobalVelocity();
    }

    /// <summary>
    /// Controller is called every frame. Use it for everything input related.
    /// </summary>
    protected virtual void ControllerTick(PlayerInput.Controller input, float dt) { }

    /// <summary>
    /// Rendering is called at the end of every frame. Use it for everything rendering related.
    /// </summary>
    protected virtual void Rendering(float dt) { }

    /// <summary>
    /// Logic tick is called more than once a frame. Use it for everything logic related.
    /// </summary>
    protected virtual void LogicTick(float dt) { }

    /// <summary>
    /// Reset current gravity velocity applyed on this character.
    /// </summary>
    public void Physic_ResetGravity()
    {
        physic.gravityVelocity = Vector3.zero;
    }

    /// <summary>
    /// Apply gravity onto this character's global velocity.
    /// </summary>
    protected void Physic_ApplyGravityOntoGlobalVelocity()
    {

        // Do not apply gravity if grounded
        if (IsGrounded())
            return;
        // Do not apply gravity if climbing
        if (IsClimbing())
            return;

        // Refresh gravity velocity by applying gravity acceleration
        physic.gravityVelocity += physic.gravity * Time.fixedDeltaTime;

        // Refresh global velocity
        physic.globalVelocity += physic.gravityVelocity;
    }

    /// <summary>
    /// Apply this character's global velocity before reseting it. Check for collisions.
    /// </summary>
    protected virtual CollisionFlags Physic_ApplyTranslationAndGlobalVelocity()
    {
        // Perform global velocity application onto the unity character controller.
        CollisionFlags collisionFlags = controller.Move(physic.translate + physic.globalVelocity * Time.fixedDeltaTime);
        // Return collision flags generated by the physical engine.
        return collisionFlags;
    }

    /// <summary>
    /// Compute collision flags generated by the aplication of the global velocity.
    /// </summary>
    /// <param name="collisionFlags">Collision flags generated by the aplication of the global velocity.</param>
    protected virtual void Physic_ComputeCollisionFlags(CollisionFlags collisionFlags)
    {
        // Check if collided with the ground
        if (
            ((collisionFlags & CollisionFlags.Below) != 0) &&
            physic.globalVelocity.y < 0
        )
        {
            // Notify that the character touched the ground
            physic.touchedGroundLastFixedUpdate = true;
            // Reset gravity
            Physic_ResetGravity();
            // Notifie collision with ground
            if (physic.eHitGround != null)
            {
                physic.eHitGround.Invoke();
            }
        }
        else
        {
            // Notify that the character didn't touched the ground
            physic.touchedGroundLastFixedUpdate = false;
        }
    }

    /// <summary>
    /// Reset this character's global velocity. (The global velocity is reset each frame after appling it.)
    /// </summary>
    protected void Physic_ResetTranslationAndGlobalVelocity()
    {
        // Clean translation
        physic.translate = Vector3.zero;
        // Clean global velocity
        physic.globalVelocity = Vector3.zero;

    }




    // GET / IS METHODS

    // Physic

    /// <summary>
    /// Returns true if this character's foot is touching the ground.
    /// </summary>
    /// <returns></returns>
    public virtual bool IsGrounded()
    {
        return physic.touchedGroundLastFixedUpdate;
    }


    // Abilities

    /// <summary>
    /// Returns true if this character is curently climbing.
    /// </summary>
    /// <returns></returns>
    public virtual bool IsClimbing()
    {
        return false;
    }

    /// <summary>
    /// Returns true if this character has it's grappin deployed.
    /// </summary>
    /// <returns></returns>
    public virtual bool IsGrabbing()
    {
        return false;
    }
}