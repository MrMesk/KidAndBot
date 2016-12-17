using UnityEngine;
using System.Collections;
using System;

public class HorizontalMobilityAbilityBot : MonoBehaviour
{
	[Header("Configuration")]
	//public float _moveSpeed = 7.5f;

	public float inputDead = 0.1f;
	public float forwardVelocity = 12;
	public float rotateVel = 100;

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
		Debug.Log("directionalVelocity : " + directionalVelocity);
		Turn();
	}

	private void FixedUpdate ()
	{
		Run();
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

	void Turn ()
	{
		if (Mathf.Abs(directionalInput.x) > inputDead)
		{
			targetRot *= Quaternion.AngleAxis(rotateVel * directionalInput.x * Time.deltaTime, Vector3.up);
			character.transform.rotation = targetRot;
		}
	}

//character.globalVelocity += directionalVelocity;
}