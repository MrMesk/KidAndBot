using UnityEngine;
using System.Collections;
using System;

namespace Abilities
{
	public class HorizontalMobilityAbilityBot : MonoBehaviour
	{
		[Header("Configuration")]
		//public float _moveSpeed = 7.5f;

		public float inputDead = 0.1f;
		public float forwardVelocity = 12;
		public float rotateVel = 100f;
		public float chargingRotateVel = 50f;
		public float chargeMaxVelMult = 2f;
		public float accelMult = 1f;
		public float slowdownMult = 2f;

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
		[Header("Sings & Feedbacks")]
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

		public Quaternion TargetRotation
		{
			get { return targetRot; }
		}

		// Use this for initialization
		void Start ()
		{
			// Retrieve required component(s)
			character = GetComponentInParent<Character>();
			if (character == null)
			{
				this.enabled = false;
				return;
			}
			targetRot = character.transform.rotation;
			//rigid = GetComponent<Rigidbody>();
		}

		// Update is called once per frame
		void Update ()
		{
			directionalInput = input.shared.directional.Value;
			//Debug.Log("directionalVelocity : " + directionalInput);
			Turn();
		}

		private void FixedUpdate ()
		{
			//Debug.Log("Velocity : " + velocity);
			RobotCharacter bot = (RobotCharacter)character;

			if(bot.isCharging)
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
				
			character.globalVelocity += character.transform.TransformDirection(velocity);

		}

		void Run ()
		{
			if (Mathf.Abs(directionalInput.y) > inputDead)
			{
				//Move
				//rigid.velocity = transform.forward * forwardInput * forwardVelocity;
				velocity.z = forwardVelocity * directionalInput.y;
			}
			else
			{
				//rigid.velocity = Vector3.zero;
				velocity.z = 0;
			}
		}

		void Charge()
		{
			// Accelerating as long as the bot is charging


			velocity.z = forwardVelocity * accel;
			//Debug.Log("Accel : " + accel);
		}

		void Turn ()
		{
			if (Mathf.Abs(directionalInput.x) > inputDead)
			{
				RobotCharacter bot = (RobotCharacter)character;
				if (bot.isCharging)
				{
					targetRot *= Quaternion.AngleAxis(chargingRotateVel * directionalInput.x * Time.deltaTime, Vector3.up);
				}
				else
				{
					targetRot *= Quaternion.AngleAxis(rotateVel * directionalInput.x * Time.deltaTime, Vector3.up);
				}
				
				character.lookRotation = targetRot;
				//character.transform.rotation = targetRot;
			}
		}

		//character.globalVelocity += directionalVelocity;
	}
}