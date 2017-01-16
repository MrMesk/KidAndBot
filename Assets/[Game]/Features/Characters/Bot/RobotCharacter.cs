using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Pro3DCamera;

public class RobotCharacter : Character
{
	[Space(10)]
	[Header("Range Parameters")]
	[Space(10)]

	public float bumpRange;
	public float bumpForwardRange;
	public float pullRange;

	[Space(10)]
	[Header("Bump Force Parameters")]
	[Space(10)]

	public float[] bumpTiers = new float[4];
	public float[] bumpForcesUp = new float[4];
	public float[] bumpForcesForward = new float[4];
	public float chargeCooldown = 2f;
	public LayerMask bumpUpMask;
	public LayerMask bumpForwardMask;
	public LayerMask destructibleWallMask;
	public Slider forceBar;

	[Space(10)]
	[Header("Camera Shaking")]

	public CameraControl cameraControl;
	[Space(10)]
	public string walkShake;
	public string shakeLow;
	public string shakeMid;
	public string shakeHigh;

	public float walkShakeFrequency;
	float walkTimer;

	[Space(10)]
	[Header("SFX")]
	[Space(10)]

	[FMODUnity.EventRef]
	public string step = "event:/Step";
	public string jump = "event:/Step";
	public string punch = "event:/Step";
	public string hookOn = "event:/Step";
	public string hookOff = "event:/Step";
	public string pull = "event:/Step";
	public string bump = "event:/Step";

	[Space(10)]
	[Header("Animation")]
	[Space(10)]

	public Animator robotAnim;
	Collider[] nearlyBlocks;

	float bumpForce;
	float chargeTimer;
	int forceTier;

	[HideInInspector]
	public bool isGrabbing;
	[HideInInspector]
	public bool isCharging;

	AnimCube grabbedCube;
	Vector3 grabbedNormal = new Vector3();

	LineRenderer grappin;
	BoxCollider grappinCollider;

	float botHeight;

	Abilities.HorizontalMobilityAbilityBot hMobility;

	protected override void LogicTick (float dt)
	{
		Physic_ApplyGravityOntoGlobalVelocity ();
	}
	// Use this for initialization
	void Start ()
	{
		Initialize();
	}

	// Update is called once per frame
	protected override void FixedUpdate ()
	{
		if (!isGrabbing)
		{
			base.FixedUpdate();
		}
	}

	protected override void Update ()
	{
		base.Update();

		ChargeManagement();

		forceBar.value = Mathf.Lerp(forceBar.value, bumpForce, 0.1f);

		InputManagement();

		if (hMobility.accel == 1 && hMobility.directionalInput != Vector2.zero && !isGrabbing/* && IsGrounded()*/)
		{
			WalkingShake();
			robotAnim.SetBool("Walking", true);
		}
		else
		{
			robotAnim.SetBool("Walking", false);
		}

	}

	/// <summary>
	/// Global input management for bot interactions
	/// </summary>
	void InputManagement()
	{
		// Attaching and detaching hook
		if (input.bot.grab.WasPressed)
		{
			if (!isGrabbing)
			{
				AttachHook();
			}
			else
			{
				DetachHook();
			}
		}
		// Cancels every other action and allows to pull if grabbing a block.
		// Also sets hook position
		if (isGrabbing)
		{
			grappin.enabled = true;
			grappin.SetPosition(0, transform.position);
			grappin.SetPosition(1, grabbedCube.transform.position);
			float dist = Vector3.Distance(transform.position, grabbedCube.transform.position) - GetComponent<CharacterController>().radius - grabbedCube.GetComponent<BoxCollider>().size.x / 2 - 1;
			if (input.bot.punch.WasReleased)
			{
				if(dist > 48f)
				{
					Pull(grabbedNormal, dist);
					isGrabbing = false;
					grabbedCube = null;
					grabbedNormal = new Vector3();
				}
				else
				{
					isGrabbing = false;
				}
			}
			
		}
		//Allows to bump and charge if not grabbing, and disables hook
		else
		{
			BumpUp();
			Charge();
			grappin.enabled = false;
			grappinCollider.enabled = false;
		}
	}

	/// <summary>
	/// Initialize all needed variables
	/// </summary>
	private void Initialize ()
	{
		chargeTimer = 0f;
		isCharging = false;
		botHeight = GetComponent<CharacterController>().height;
		hMobility = GetComponentInChildren<Abilities.HorizontalMobilityAbilityBot>();

		grappin = transform.Find("Rendering").Find("Grappin").GetComponent<LineRenderer>();
		grappinCollider = transform.Find("Rendering").Find("GrappinCollider").GetComponent<BoxCollider>();

		grappin.enabled = false;
		grappinCollider.enabled = false;

		if (forceBar == null)
		{
			forceBar = GameObject.Find("ForceBar").GetComponent<Slider>();
		}
	}

	/// <summary>
	/// Little Camera shake to simulate heavy walking
	/// </summary>
	void WalkingShake ()
	{
		walkTimer += Time.deltaTime;
		if (walkTimer >= walkShakeFrequency)
		{
			walkTimer = 0f;
			//Debug.Log ("Cam Control : " + cameraControl.name);
			cameraControl.ShakeCamera(walkShake);
			FMODUnity.RuntimeManager.PlayOneShot(step, transform.position);
		}
	}

	/// <summary>
	/// Power amount management. If any input is held, the charge bar goes up. Else, the charge bar progressively goes down
	/// </summary>
	void ChargeManagement ()
	{

		if (input.bot.punch.IsPressed || input.bot.pull.IsPressed || input.bot.bump.IsPressed)
		{
			//Debug.Log("Charging");
			bumpForce += Time.deltaTime;
			bumpForce = Mathf.Clamp(bumpForce, 0f, bumpTiers[3] * 1.25f);
		}
		else
		{

			bumpForce -= Time.deltaTime;
			bumpForce = Mathf.Clamp(bumpForce, 0f, bumpTiers[3] * 1.25f);
		}
	}

	/// <summary>
	/// Sets force depending on charge bar
	/// </summary>
	void ForceCheck ()
	{
		if (bumpForce >= bumpTiers[3])
		{
			forceTier = 3;
		}
		else if (bumpForce >= bumpTiers[2])
		{
			forceTier = 2;
		}
		else if (bumpForce >= bumpTiers[1])
		{
			forceTier = 1;
		}
		else
		{
			forceTier = 0;
		}

		bumpForce = 0f;
	}

	#region Charging

	/// <summary>
	/// Sets charging state if input held by player and cooldown is available
	/// </summary>
	void Charge ()
	{
		if (input.bot.punch.IsPressed && chargeTimer == 0f)
		{
			robotAnim.SetBool("Charging", true);
			robotAnim.SetBool("Walking", false);
			isCharging = true;
		}
		else
		{
			robotAnim.SetBool("Charging", false);
			isCharging = false;
		}
	}

	/// <summary>
	/// On Impact, resets the cooldown Timer so the player can't spam the charge.
	/// </summary>
	/// <returns></returns>
	public IEnumerator ChargeCooldown ()
	{
		chargeTimer = chargeCooldown;

		while (chargeTimer > 0f)
		{
			chargeTimer -= Time.deltaTime;
			//Debug.Log("Charge Timer : " + chargeTimer);
			yield return null;
		}
		chargeTimer = 0f;
		yield return null;
	}

	#endregion

	#region HookManagement

	void DetachHook ()
	{
		grabbedCube = null;
		isGrabbing = false;
		grappinCollider.enabled = false;
		FMODUnity.RuntimeManager.PlayOneShot(hookOff, transform.position);
	}

	void PlaceGrapCollider (Vector3 targetPoint)
	{
		grappinCollider.enabled = true;
		Vector3 toCube = grabbedCube.transform.position - transform.position;
		grappinCollider.transform.position = transform.position + (toCube) / 2 - toCube.normalized * (grabbedCube.transform.GetComponent<BoxCollider>().size.x / 4f);
		Vector3 newSize = new Vector3(16f, 16f, (transform.position - targetPoint).magnitude * 3);

		//Debug.Log("Attaching hook");
		Ray ray = new Ray(transform.position, transform.forward);

		grappinCollider.size = newSize;
		grappinCollider.transform.forward = (grabbedCube.transform.position - grappinCollider.transform.position);
	}

	void AttachHook ()
	{
		//Debug.Log("Attaching hook");
		Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, pullRange, bumpForwardMask))
		{
			Vector3 cubeNormal = hit.normal;
			AnimCube animCube;

			if (Vector3.Dot((transform.position - hit.point), cubeNormal) > 0.5)
			{
				animCube = hit.transform.GetComponent<AnimCube>();
				if (animCube.bumping == false)
				{
					grabbedNormal = cubeNormal;
					grabbedCube = animCube;
					PlaceGrapCollider(hit.point);
					isGrabbing = true;
				}
			}

			FMODUnity.RuntimeManager.PlayOneShot(hookOn, transform.position);
		}
	}

	public override bool IsGrabbing ()
	{
		return isGrabbing;
	}

	#endregion

	#region Level Module Movement


	/// <summary>
	/// Pulls a Level Module towards the Bot, depending on the force
	/// </summary>
	/// <param name="dir">Direction the Level Module is pulled towards</param>
	/// <param name="distToLM">Distance between Bot and Level Module so the bot can't pull too far</param>
	void Pull (Vector3 dir, float distToLM)
	{
		ForceCheck();

		while (bumpForcesForward[forceTier] > distToLM)
		{
			if (forceTier >= 1)
			{
				forceTier--;
			}
			else
			{
				break;
			}
		}
		AnimCube basis;

		if (grabbedCube.linked == true)
		{
			basis = grabbedCube.GetBasis();
		}
		else
		{
			basis = grabbedCube;
		}

		if (basis.IsAgainstWall(dir))
		{
			if (basis.transform.GetComponentInChildren<AnimCube>().IsAgainstWall(dir))
			{
				basis = grabbedCube;
				basis.transform.parent = GameObject.FindGameObjectWithTag("LevelContainer").transform;
			}

			basis.transform.GetComponentInChildren<AnimCube>().transform.parent = basis.transform.parent;
			basis = grabbedCube.GetBasis();
		}

		basis.BumpingToDir(bumpForcesForward[forceTier], dir);

		cameraControl.ShakeCamera(shakeMid);
		FMODUnity.RuntimeManager.PlayOneShot(pull, transform.position);
	}

	/// <summary>
	/// Bumps a Level Module depending on the normal collided with the bot
	/// </summary>
	/// <param name="anim">The collided cube AnimCube component</param>
	void PushLM (AnimCube anim)
	{
		Ray ray = new Ray(transform.position, anim.transform.position - transform.position);
		RaycastHit hit;

		ForceCheck();
		robotAnim.SetBool("Charging", false);
		//Debug.DrawRay(ray.origin, ray.direction * 20f, Color.red);
		if (Physics.Raycast(ray, out hit, bumpForwardRange, bumpForwardMask))
		{
			//Debug.Log ("Raycast hits !!");
			Vector3 cubeNormal = hit.normal;

			if (anim.bumping == false)
			{
				GameObject particleImpact = Resources.Load("Particles/Impact" + forceTier) as GameObject;
				particleImpact = Instantiate(particleImpact, hit.point, Quaternion.identity) as GameObject;
				particleImpact.transform.forward = cubeNormal;

				AnimCube basis;
				Debug.Log("Is Collided cube linked ? " + anim.linked);
				if (anim.linked == true)
				{
					basis = anim.GetBasis();
					Debug.Log("Is Basis" + basis.name + " linked ? " + basis.linked);
				}
				else
				{
					basis = anim;
				}
				Debug.Log("Basis : " + basis.name);
				if (basis.IsAgainstWall(-cubeNormal))
				{
					if (basis.transform.GetComponentInChildren<AnimCube>().IsAgainstWall(-cubeNormal))
					{
						basis = anim;
						basis.transform.parent = GameObject.FindGameObjectWithTag("LevelContainer").transform;
					}

					basis.transform.GetComponentInChildren<AnimCube>().transform.parent = basis.transform.parent;
					basis = anim.GetBasis();
				}

				basis.BumpingToDir(bumpForcesForward[forceTier], -cubeNormal);
			}
		}

		cameraControl.ShakeCamera(shakeMid);
		FMODUnity.RuntimeManager.PlayOneShot(punch, transform.position);
	}

	/// <summary>
	/// Bumps every block around impact point as long as they belong to the bumpable layer. Bumping force depends on charge bar
	/// </summary>
	void BumpUp ()
	{
		if (input.bot.bump.WasReleased && IsGrounded())
		{
			ForceCheck();
			robotAnim.SetTrigger("Bump");
			GameObject particleImpact = Resources.Load("Particles/ImpactGround") as GameObject;
			particleImpact = Instantiate(particleImpact, transform.position - new Vector3(0, botHeight / 2, 0), Quaternion.identity) as GameObject;
			particleImpact.transform.forward = Vector3.up;

			AnimCube animCube;

			//Getting every block in range, but not the ones below the player and bumping them according to the bumping force
			nearlyBlocks = Physics.OverlapSphere(transform.position - new Vector3(0,botHeight/2,0), bumpRange, bumpUpMask);
			
			foreach (Collider col in nearlyBlocks)
			{
				//Debug.Log("Pos X Player : " + (transform.position.y - transform.localScale.y) + "Pos X Target : " + (col.transform.position.y - col.transform.localScale.y / 2));
				animCube = col.GetComponent<AnimCube>();
				if (col.transform.position.y - col.transform.localScale.y / 2 >= transform.position.y - botHeight / 2 - 4f && animCube.bumping == false)
				{
					animCube.StartCoroutine(animCube.BumpUp(2f, bumpForcesUp[forceTier]));
				}
			}
			cameraControl.ShakeCamera(shakeMid);
			FMODUnity.RuntimeManager.PlayOneShot(bump, transform.position);

			// Bump kid
			TryBumpKid();
		}
	}

	/// <summary>
	/// Tries bumping the kid if he's nearby
	/// </summary>
	/// <returns></returns>
	public bool TryBumpKid ()
	{
		GameObject kid = GameObject.Find("Kid");
		if (kid == null)
		{ return false; } // Kid not found
		float distToKid = Vector3.Distance(transform.position - new Vector3(0, botHeight / 2, 0), kid.transform.position);
		if (distToKid > bumpRange)
		{ return false; } // Kid too far
		Abilities.JumpAbility jumpAbility = kid.GetComponentInChildren<Abilities.JumpAbility>();
		if (jumpAbility == null)
		{ return false; } // Kid can't jump
		jumpAbility.ForceJumpRequest(bumpForcesUp[forceTier] / 2);
		return true; // Succesfully forced kid to jump
	}

	#endregion

	/// <summary>
	/// Impacts any obstacle or any Level Module when charging.
	/// If the obstacle gets destroyed, the bot keeps charging.
	/// </summary>
	/// <param name="other">Returns any obstacle the Bot collides with</param>
	private void OnTriggerEnter (Collider other)
	{
		if (isCharging)
		{
			Destructible prop = other.GetComponent<Destructible>();
			if (prop != null)
			{
				Debug.Log ("Colliding with something !");
				if (prop.Impact() == false)
				{
					robotAnim.SetBool("Charging", false);
					isCharging = false;
					hMobility.accel = 1f;
					StartCoroutine(ChargeCooldown());
				}

				AnimCube anim = other.GetComponent<AnimCube>();
				if (anim != null) 
				{
					Debug.Log("Colliding with a Level Module !");
					PushLM (anim);
				} 
				else 
				{
					DestructibleWall wall = other.GetComponent<DestructibleWall> ();
					if (wall != null) 
					{
						ForceCheck();
						wall.Destruct (transform.forward, bumpForcesForward[forceTier] * 10f);
					}
				}
			}
		}
	}
}
