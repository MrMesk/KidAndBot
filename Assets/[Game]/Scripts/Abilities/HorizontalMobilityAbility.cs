﻿using UnityEngine;
using System.Collections;
using System;

namespace Abilities {
    
    public partial class HorizontalMobilityAbility : MonoBehaviour
	{
        // Configuration
        [Header("Configuration")]
        public float _moveSpeed = 7.5f;
		public float _toBotSpeedMult = 1.2f;
		[Range(-1f,1f)]
		public float _minDotToBot = 0.9f;

        public enum DebugInputMode
		{
            NONE,
            GAMEPAD,
            KEYBOARD,
            BOTH
        }

        // Debug
        [Header("Debug")]
        public DebugInputMode debugInputMode = DebugInputMode.BOTH;

        // S&F
        [Header("Sings & Feedbacks")]
        [Range(0,1)]
        public float editLookDirectionDeadZone = 0.1f;


        // State
        [NonSerialized] private Character character;
        [NonSerialized] public Vector2 directionalInput;
        // Input
        private PlayerInput.Controller input { get { return character.input; } }

        public Vector3 directionalVelocity { get; private set; }

		Transform bot;

        private void Start()
		{
            // Retrieve required component(s)
            character = GetComponentInParent<Character>();
			bot = FindObjectOfType<RobotCharacter>().transform;

			if (character == null)
			{
                this.enabled = false;
                return;
            }
        }

        public void Update()
		{
            directionalInput = input.shared.directional.Value;
        }

        public void LateUpdate()
		{
            // Refresh look rotation
            if (directionalVelocity.magnitude > editLookDirectionDeadZone)
			{
                character.lookRotation = Quaternion.LookRotation(directionalVelocity, -character.physic.gravity);
            }
        }

        private void FixedUpdate()
		{
            // TODO : True climbing ability
            // Do not walk if climbing
            //if (character.IsClimbing())
            //    return;

            // Do not walk if grabing
            if (character.IsGrabbing())
                return;

            // Directional
            directionalVelocity = GetDirectionalVelocity();
            
            // Apply to global velocity
            character.physic.globalVelocity += directionalVelocity;
        }

        protected Vector3 GetDirectionalVelocity()
		{
            // Directional
            Quaternion forwardRotation = character.characterCompass.transform.rotation;
            Vector3 directional = Vector3.ClampMagnitude(new Vector3(directionalInput.x, 0, directionalInput.y), 1);
            directional = forwardRotation * directional;

			Vector3 toBot = bot.position - transform.position;
			float delta = Vector3.Dot(toBot.normalized, directional.normalized);

			if (delta > _minDotToBot)
			{
				directional *= _moveSpeed * _toBotSpeedMult;
			}
			else
			{
				directional *= _moveSpeed;
			}
            return directional;
        }
    }
}
