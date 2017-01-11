﻿using UnityEngine;
using System.Collections;
using System;

namespace Abilities {
    public class JumpAbility : MonoBehaviour {

        [Header("Configuration")]
        // Config
        public float maxJumpHeight = 4;
        public float minJumpHeight = 1;
        public float timeToJumpApex = .4f;
        public float airFriction = 50.0f;

        // State
        private Character character;

        private float maxJumpVelocity;
        private float minJumpVelocity;

        private Vector3 jumpVelocity = Vector3.zero;

        private bool madeJumpRequest = false;
        private bool madeStopJumpRequest = false;
        
        // Input
        private PlayerInput.Controller input { get { return character.input; } }

        void Start() {
            // Retrieve required component(s)
            character = GetComponentInParent<Character>();
            if (character == null) {
                this.enabled = false;
                return;
            }

            // Initialise jump
            InitialiseJump();
        }

        private void InitialiseJump() {
            // Setup jump
            character.physic.gravity.y = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            maxJumpVelocity = Mathf.Abs(character.physic.gravity.y) * timeToJumpApex;
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(character.physic.gravity.y) * minJumpHeight);

            // Bind handler(s)
            character.physic.eHitGround += OnHitGround;
        }

        // Update is called once per frame
        void Update() {
            // Input

            // Jump request
            madeJumpRequest |= input.kid.jump.WasPressed;
            madeJumpRequest &= input.kid.jump.IsPressed;

            // Stop jump request
            madeStopJumpRequest |= input.kid.jump.WasReleased;
            madeStopJumpRequest &= !input.kid.jump.IsPressed;
        }

        void FixedUpdate() {
            // Check for jump request
            if (madeJumpRequest) {
                if (character.IsGrounded()) {
                    // Regular jump if on ground
                    Jump();
                } else if (character.IsClimbing()) {
                    // Wall jump if climbing
                    WallJump();
                }
            }

            // Check for stop jump request
            float yVel = character.physic.gravityVelocity.y + jumpVelocity.y;
            if (madeStopJumpRequest && yVel > minJumpVelocity) {
                jumpVelocity.y = minJumpVelocity;
                madeStopJumpRequest = false;
            }

            // Apply lateral friction
            jumpVelocity = Vector3.MoveTowards(jumpVelocity, Vector3.Project(jumpVelocity, Vector2.up), airFriction * Time.fixedDeltaTime);

            // Apply to global velocity
            character.physic.globalVelocity += jumpVelocity;

        }

        // Event handlers    
        private void OnHitCeiling() {
            ResetJump();
            character.Physic_ResetGravity();
        }

        private void OnHitGround() {
            ResetJump();
        }

        public void OnDestroy() {
            // Unbind jump reset
            character.physic.eHitGround -= OnHitGround;
        }

        // Logic

        public void Jump() {
            character.Physic_ResetGravity();
            jumpVelocity.y = maxJumpVelocity;
            madeJumpRequest = false;
            madeStopJumpRequest = false;
        }

        public void WallJump() {
            character.Physic_ResetGravity();
            jumpVelocity = Vector3.RotateTowards(character.characterCompass.transform.up, Vector3.up, 45 * Mathf.Deg2Rad, 0) * maxJumpVelocity;
            madeJumpRequest = false;
            madeStopJumpRequest = false;
        }

        public void ResetJump() {
            jumpVelocity = Vector3.zero;
        }

        public void ForceJumpRequest(float bumpForce)
		{
			if(jumpVelocity != Vector3.zero)
			{
				character.Physic_ResetGravity();
				jumpVelocity.y = bumpForce;
				madeJumpRequest = true;
				madeStopJumpRequest = false;
			}
        }

    }
}