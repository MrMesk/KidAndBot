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

        // State
        private Character character;

        private float maxJumpVelocity;
        private float minJumpVelocity;

        private Vector3 jumpVelocity = Vector3.zero;

        private bool madeJumpRequest = false;
        private bool madeStopJumpRequest = false;



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
            character.gravity.y = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            maxJumpVelocity = Mathf.Abs(character.gravity.y) * timeToJumpApex;
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(character.gravity.y) * minJumpHeight);

            // Bind handler(s)
            character.eHitGround += OnHitGround;
        }

        // Update is called once per frame
        void Update() {
            // Input

            // Jump request
            madeJumpRequest |= Input.GetButtonDown("Jump");
            madeJumpRequest &= !Input.GetButtonUp("Jump");

            // Stop jump request
            madeStopJumpRequest |= Input.GetButtonUp("Jump");
            madeStopJumpRequest &= !Input.GetButtonDown("Jump");
        }

        void FixedUpdate() {
            // Check for jump request
            if (character.IsGrounded() && madeJumpRequest) {
                Jump();
            }

            // Check for stop jump request
            float yVel = character.gravityVelocity.y + jumpVelocity.y;
            if (madeStopJumpRequest && yVel > minJumpVelocity) {
                jumpVelocity.y = minJumpVelocity;
                madeStopJumpRequest = false;
            }

            // Apply to global velocity
            character.globalVelocity += jumpVelocity;

        }

        // Event handlers    
        private void OnHitCeiling() {
            ResetJump();
            character.ResetGravity();
        }

        private void OnHitGround() {
            ResetJump();
        }

        public void OnDestroy() {
            // Unbind jump reset
            character.eHitGround -= OnHitGround;
        }

        // Logic

        public void Jump() {
            character.ResetGravity();
            jumpVelocity.y = maxJumpVelocity;
            madeJumpRequest = false;
            madeStopJumpRequest = false;
        }

        public void ResetJump() {
            jumpVelocity = Vector3.zero;
        }

        public void ForceJumpRequest() {
            madeJumpRequest = true;
            madeStopJumpRequest = false;
        }

    }
}