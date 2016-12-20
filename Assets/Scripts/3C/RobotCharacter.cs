using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

	public CameraShake cameraShaker;
	[Space(10)]
	public float walkShakeFrequency;
	public float walkShakeDuration;
	public float walkShakeMagnitude;
	float walkTimer;
	[Space(10)]
	public float[] shakeTiersMagnitude = new float[4];
	public float[] shakeTiersDuration = new float[4];

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


	bool isGrabbing;
	[HideInInspector]
	public bool isCharging;

	AnimCube grabbedCube;
	Vector3 grabbedNormal = new Vector3();

	LineRenderer grappin;
	BoxCollider grappinCollider;

	float botHeight;

	Abilities.HorizontalMobilityAbilityBot hMobility;

	// Use this for initialization
	void Start ()
	{
		chargeTimer = 0f;
		isCharging = false;
		botHeight = GetComponent<CharacterController>().height;
		hMobility = GetComponentInChildren<Abilities.HorizontalMobilityAbilityBot>();

		grappin = transform.Find("Rendering").Find("Grappin").GetComponent<LineRenderer>();
		grappinCollider = transform.Find("Rendering").Find("GrappinCollider").GetComponent<BoxCollider>();

		if (cameraShaker == null)
		{
			cameraShaker = characterCamera.transform.Find("BotCam").GetComponent<CameraShake>();
		}
		grappin.enabled = false;
		grappinCollider.enabled = false;

		if (forceBar == null)
		{
			forceBar = GameObject.Find("ForceBar").GetComponent<Slider>();
		}
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
		if (isGrabbing)
		{
			grappin.enabled = true;
			grappin.SetPosition(0, transform.position);
			grappin.SetPosition(1, grabbedCube.transform.position);
			float dist = Vector3.Distance(transform.position, grabbedCube.transform.position) - GetComponent<CharacterController>().radius - grabbedCube.GetComponent<BoxCollider>().size.x / 2 - 1;
			//Debug.Log("Distance to LM : " + dist);
			//Debug.Log("Character size" + GetComponent<CharacterController>().radius);
			if (input.bot.punch.WasReleased && dist > 48f)
			{
				Pull(grabbedNormal, dist);
				isGrabbing = false;
				grabbedCube = null;
				grabbedNormal = new Vector3();
			}
		}
		else
		{
			BumpUp();
			//BumpForward();
			Charge();
			grappin.enabled = false;
		}

		//Debug.Log("Directional Input" + _directionalInput);
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

	void WalkingShake ()
	{
		walkTimer += Time.deltaTime;
		if (walkTimer >= walkShakeFrequency)
		{
			walkTimer = 0f;
			cameraShaker.StartCoroutine(cameraShaker.Shake(walkShakeDuration, walkShakeMagnitude));
			FMODUnity.RuntimeManager.PlayOneShot(step, transform.position);
		}
	}
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

	void Charge ()
	{
		if (input.bot.punch.IsPressed && chargeTimer == 0f)
		{
			isCharging = true;
		}
		else
		{
			isCharging = false;
		}
	}
	void BumpForward ()
	{
		if (input.bot.punch.WasReleased)
		{
			Ray ray = new Ray(transform.position, transform.forward);
			RaycastHit hit;

			ForceCheck();
			robotAnim.SetTrigger("Punching");
			//Debug.DrawRay(ray.origin, ray.direction * 20f, Color.red);
			if (Physics.Raycast(ray, out hit, bumpForwardRange, bumpForwardMask))
			{
				//Debug.Log ("Raycast hits !!");
				Vector3 cubeNormal = hit.normal;
				AnimCube animCube;
				if (Vector3.Dot(cubeNormal, hit.transform.forward) > 0.5f ||
					Vector3.Dot(cubeNormal, -hit.transform.forward) > 0.5f ||
					Vector3.Dot(cubeNormal, hit.transform.right) > 0.5f ||
					Vector3.Dot(cubeNormal, -hit.transform.right) > 0.5f)
				{
					animCube = hit.transform.GetComponent<AnimCube>();
					if (animCube.bumping == false)
					{
						GameObject particleImpact = Resources.Load("Particles/Impact" + forceTier) as GameObject;
						particleImpact = Instantiate(particleImpact, hit.point, Quaternion.identity) as GameObject;
						particleImpact.transform.forward = cubeNormal;
						//animCube.StartCoroutine (animCube.BumpToDir (2f, bumpForcesForward [forceTier], -cubeNormal));

						AnimCube basis;
						Debug.Log("Is Collided cube linked ? " + animCube.linked);
						if (animCube.linked == true)
						{
							basis = animCube.GetBasis();
							Debug.Log("Is Basis" + basis.name + " linked ? " + basis.linked);
						}
						else
						{
							basis = animCube;
						}
						Debug.Log("Basis : " + basis.name);
						if (basis.IsAgainstWall(-cubeNormal))
						{
							if (basis.transform.GetComponentInChildren<AnimCube>().IsAgainstWall(-cubeNormal))
							{
								basis = animCube;
								basis.transform.parent = GameObject.FindGameObjectWithTag("LevelContainer").transform;
							}

							basis.transform.GetComponentInChildren<AnimCube>().transform.parent = basis.transform.parent;
							basis = animCube.GetBasis();
						}

						basis.BumpingToDir(bumpForcesForward[forceTier], -cubeNormal);
						//animCube.BumpingToDir(bumpForcesForward[forceTier], -cubeNormal);
					}
				}
				cameraShaker.StartCoroutine(cameraShaker.Shake(shakeTiersDuration[forceTier], shakeTiersMagnitude[forceTier]));
				FMODUnity.RuntimeManager.PlayOneShot(punch, transform.position);
			}

			else if (Physics.Raycast(ray, out hit, bumpForwardRange, destructibleWallMask))
			{
				//Debug.Log ("Raycast hits !!");
				Vector3 cubeNormal = hit.normal;
				DestructibleWall destructibleWall;
				if (Vector3.Dot(cubeNormal, hit.transform.forward) > 0.5f ||
					Vector3.Dot(cubeNormal, -hit.transform.forward) > 0.5f ||
					Vector3.Dot(cubeNormal, hit.transform.right) > 0.5f ||
					Vector3.Dot(cubeNormal, -hit.transform.right) > 0.5f)
				{
					destructibleWall = hit.transform.GetComponent<DestructibleWall>();

					GameObject particleImpact = Resources.Load("Particles/Impact" + forceTier) as GameObject;
					particleImpact = Instantiate(particleImpact, hit.point, Quaternion.identity) as GameObject;
					particleImpact.transform.forward = cubeNormal;
					destructibleWall.Destruct(-cubeNormal, bumpForcesForward[forceTier] * 20f);
				}
				cameraShaker.StartCoroutine(cameraShaker.Shake(shakeTiersDuration[forceTier], shakeTiersMagnitude[forceTier]));
				FMODUnity.RuntimeManager.PlayOneShot(punch, transform.position);
			}
		}
	}

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

		cameraShaker.StartCoroutine(cameraShaker.Shake(shakeTiersDuration[forceTier], shakeTiersMagnitude[forceTier]));
		FMODUnity.RuntimeManager.PlayOneShot(pull, transform.position);
	}

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
		Vector3 newSize = new Vector3(4f, 4f, (transform.position - targetPoint).magnitude * 3);

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

	void BumpUp ()
	{
		if (input.bot.bump.WasReleased && IsGrounded())
		{
			ForceCheck();

			GameObject particleImpact = Resources.Load("Particles/ImpactGround") as GameObject;
			particleImpact = Instantiate(particleImpact, transform.position - new Vector3(0, botHeight / 2, 0), Quaternion.identity) as GameObject;
			particleImpact.transform.forward = Vector3.up;

			AnimCube animCube;

			//Getting every block in range, but not the ones below the player and bumping them according to the bumping force
			nearlyBlocks = Physics.OverlapSphere(transform.position, bumpRange, bumpUpMask);
			foreach (Collider col in nearlyBlocks)
			{
				//Debug.Log("Pos X Player : " + (transform.position.y - transform.localScale.y) + "Pos X Target : " + (col.transform.position.y - col.transform.localScale.y / 2));
				animCube = col.GetComponent<AnimCube>();
				if (col.transform.position.y - col.transform.localScale.y / 2 >= transform.position.y - botHeight / 2 - 4f && animCube.bumping == false)
				{
					animCube.StartCoroutine(animCube.BumpUp(2f, bumpForcesUp[forceTier]));
				}
			}
			cameraShaker.StartCoroutine(cameraShaker.Shake(shakeTiersDuration[forceTier], shakeTiersMagnitude[forceTier]));
			FMODUnity.RuntimeManager.PlayOneShot(bump, transform.position);

			// Bump kid
			TryBumpKid();
		}
	}

	public bool TryBumpKid ()
	{
		GameObject kid = GameObject.Find("Kid");
		if (kid == null)
		{ return false; } // Kid not found
		float distToKid = Vector3.Distance(transform.position, kid.transform.position);
		if (distToKid > bumpRange)
		{ return false; } // Kid too far
		Abilities.JumpAbility jumpAbility = kid.GetComponentInChildren<Abilities.JumpAbility>();
		if (jumpAbility == null)
		{ return false; } // Kid can't jump
		jumpAbility.ForceJumpRequest();
		return true; // Succesfully forced kid to jump
	}

	public override bool IsGrabbing ()
	{
		return isGrabbing;
	}

	private void OnTriggerEnter (Collider other)
	{
		if (isCharging)
		{
			Destructible prop = other.GetComponent<Destructible>();
			if (prop != null)
			{
				
				if (prop.Impact() == false)
				{
					isCharging = false;
					hMobility.accel = 1f;
					StartCoroutine(ChargeCooldown());
				}

				AnimCube anim = other.GetComponent<AnimCube>();
				if (anim != null)
				{
					Debug.Log("Colliding with a Level Module !");
					PushLM(anim);
				}
			}
		}
	}

	void PushLM (AnimCube anim)
	{
		Ray ray = new Ray(transform.position, anim.transform.position - transform.position);
		RaycastHit hit;

		ForceCheck();
		robotAnim.SetTrigger("Punching");
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
		cameraShaker.StartCoroutine(cameraShaker.Shake(shakeTiersDuration[forceTier], shakeTiersMagnitude[forceTier]));
		FMODUnity.RuntimeManager.PlayOneShot(punch, transform.position);
	}

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

}
