using UnityEngine;
using System.Collections;
using System;

namespace Abilities
{
	public class HorizontalMobilityAbilityBot : MonoBehaviour
	{
		[Header("Configuration")]
		[Range(0f,1f)]
		public float inputDead = 0.1f; // Classic input dead, nullifies input values below that 
		public float forwardVelocity = 12; // Default walking speed
		public float rotateVel = 100f; // Rotation speed value when in running state
		public float chargingRotateVel = 50f; // Rotation speed value when in charging state
		[Range(1f, 10f)]
		public float chargeMaxVelMult = 2f; // Max value for accel
		[Range(1f, 10f)]
		public float accelMult = 1f; // How fast does the bot reach maximum acceleration ! (Higher = faster)
		[Range(1f, 10f)]
		public float slowdownMult = 2f; // How fast does the bot reach minimum acceleration ! (Higher = faster)
		[Range(1f, 2f)]
		public float toKidSpeedMult = 1.5f;
		[Range(0.1f, 1f)]
		public float backwardSpeedMult = 0.5f;
		[Range(-1f, 1f)]
		public float _minDotToKid = 0.9f;

		[HideInInspector] public float accel = 1f;
		Vector3 velocity = Vector3.zero;

		Quaternion targetRot;

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
		[Header("Signs & Feedbacks")]
		[Range(0, 1)]
		public float editLookDirectionDeadZone = 0.1f;

		// State
		[NonSerialized]
		private Character character;
		[NonSerialized]
		public Vector2 directionalInput;
		// Input
		private PlayerInput.Controller input { get { return character.input; } }

		public Vector3 directionalVelocity { get; private set; }

        [Header("KID")]
        public Gameplay.KidCharacter kidCharacter;

		public Quaternion TargetRotation
		{
			get { return targetRot; }
		}

		Transform kid;
		// Use this for initialization
		void Start ()
		{
			// Retrieve required component(s)
			kid = FindObjectOfType<Gameplay.KidCharacter>().transform;
			character = GetComponentInParent<Character>();
			if (character == null)
			{
				this.enabled = false;
				return;
			}
			targetRot = character.transform.rotation;
		}

		// Update is called once per frame
		void Update ()
		{
			directionalInput = input.shared.directional.Value;
			Turn();
		}

		private void FixedUpdate ()
		{
			ChargeManagement();
		}

		/// <summary>
		/// Checks if the bot is currently in charge state, and increases/decreases acceleration if true/false. Allows to walk as long as accel is equal to 1
		/// </summary>
		void ChargeManagement()
		{
			//Debug.Log("Velocity : " + velocity);
			RobotCharacter bot = (RobotCharacter)character;

			if (bot.isCharging)
			{
				accel += Time.deltaTime * accelMult;
				accel = Mathf.Clamp(accel, 1f, chargeMaxVelMult);
			}
			else
			{
				//Lowering charge acceleration, adding 
				accel -= Time.deltaTime * slowdownMult;
				accel = Mathf.Clamp(accel, 1f, chargeMaxVelMult);
			}

			if (accel > 1f)
			{
				Charge();
			}
			else
			{
				Run();
			}

			character.physic.globalVelocity += character.transform.TransformDirection(velocity);
            if (kidCharacter.transform.IsChildOf(character.transform))
            {
                kidCharacter.physic.globalVelocity += character.transform.TransformDirection(velocity);
            }
		}

		/// <summary>
		/// Classic walking mode. Allows to go Forward/Backwards, going faster if going towards the kid.
		/// </summary>
		void Run ()
		{
			RobotCharacter bot = (RobotCharacter)character;
			
			if (Mathf.Abs(directionalInput.y) > inputDead && bot.isGrabbing == false)
			{
				//Move
				if(directionalInput.y > 0f)
				{
					Vector3 toKid = kid.position - transform.position;
					toKid.Normalize();

					float delta = Vector3.Dot(character.transform.forward, toKid);

					if (delta > _minDotToKid)
					{
						velocity.z = forwardVelocity * directionalInput.y * toKidSpeedMult;
					}
					else
					{
						velocity.z = forwardVelocity * directionalInput.y;
					}
				}
				else
				{
					velocity.z = forwardVelocity * directionalInput.y * backwardSpeedMult;
				}
				
			}
			else
			{
				//rigid.velocity = Vector3.zero;
				velocity.z = 0;
			}
		}

		/// <summary>
		/// Charging mode. Goes forward as long as isCharging is set to true and accel is > 1.
		/// </summary>
		void Charge()
		{
			// Accelerating as long as the bot is charging
			velocity.z = forwardVelocity * accel;
		}

		/// <summary>
		/// Allows bot to rotate left or right. Rotation speed is lowered during charging state
		/// </summary>
		void Turn ()
		{
			RobotCharacter bot = (RobotCharacter)character;
			if (Mathf.Abs(directionalInput.x) > inputDead && bot.isGrabbing == false)
			{
				if (bot.isCharging)
				{
					targetRot *= Quaternion.AngleAxis(chargingRotateVel * directionalInput.x * Time.deltaTime, Vector3.up);
				}
				else
				{
					targetRot *= Quaternion.AngleAxis(rotateVel * directionalInput.x * Time.deltaTime, Vector3.up);
				}
				
				character.lookRotation = targetRot;
			}
		}
	}
}