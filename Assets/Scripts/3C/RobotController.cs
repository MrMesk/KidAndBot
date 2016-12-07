﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RobotController : Character
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
	int forceTier;
    

    bool isGrabbing;
	AnimCube grabbedCube;
	Vector3 grabbedNormal = new Vector3();

	LineRenderer grappin;
	BoxCollider grappinCollider;

	float botHeight;

	Abilities.HorizontalMobilityAbility hMobility;

	// Use this for initialization
	void Start ()
	{
<<<<<<< HEAD:Assets/Scripts/3C/RobotController.cs
		botHeight = GetComponent<CharacterController> ().height;
		hMobility = GetComponentInChildren<Abilities.HorizontalMobilityAbility>();
		grappin = GameObject.Find("Grappin").GetComponent<LineRenderer>();
		grappinCollider = GameObject.Find("GrappinCollider").GetComponent<BoxCollider>();

=======
//<<<<<<< HEAD
		//grappin = transform.FindChild("Grappin").GetComponent<LineRenderer>();
//=======
		grappin = transform.Find("Rendering").Find("Grappin").GetComponent<LineRenderer>();
		grappinCollider = transform.Find("Rendering").Find("GrappinCollider").GetComponent<BoxCollider>();
//>>>>>>> origin/Robot
>>>>>>> 4f570ff25fa5cee53cdffd2a8159094ca94a2001:Assets/Scripts/RobotController.cs

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
<<<<<<< HEAD:Assets/Scripts/3C/RobotController.cs
			//Debug.Log("Right Mouse Click");
			if (!isGrabbing) {
=======
			Debug.Log("Is Grabbing : " + isGrabbing);
			if (!isGrabbing)
			{
>>>>>>> 4f570ff25fa5cee53cdffd2a8159094ca94a2001:Assets/Scripts/RobotController.cs
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

			if (input.bot.punch.WasReleased)
			{
				Pull(grabbedNormal);
				isGrabbing = false;
				grabbedCube = null;
				grabbedNormal = new Vector3();
			}
		}
		else
		{
			BumpUp();
			BumpForward();
			grappin.enabled = false;
		}

		//Debug.Log("Directional Input" + _directionalInput);
		if (hMobility.directionalInput != Vector2.zero && !isGrabbing/* && IsGrounded()*/)
		{
			WalkingShake();
			robotAnim.SetBool("Walking", true);
		}
<<<<<<< HEAD:Assets/Scripts/3C/RobotController.cs
		else
		{
			robotAnim.SetBool("Walking", false);
		}

=======
		/*
		if (Input.GetButtonDown("Jump"))
		{
			cameraShaker.StartCoroutine(cameraShaker.Shake(walkShakeDuration * 2, walkShakeMagnitude * 2));
			GameObject particleImpact = Resources.Load("Particles/ImpactJump") as GameObject;
			particleImpact = Instantiate(particleImpact, transform.position - new Vector3(0, transform.localScale.y / 2, 0), Quaternion.identity) as GameObject;
			particleImpact.transform.forward = Vector3.up;

			FMODUnity.RuntimeManager.PlayOneShot(jump, transform.position);
		}*/
>>>>>>> 4f570ff25fa5cee53cdffd2a8159094ca94a2001:Assets/Scripts/RobotController.cs
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
						Debug.Log ("Is Collided cube linked ? " + animCube.linked);
						if (animCube.linked == true) 
						{
							basis = animCube.GetBasis ();
							Debug.Log ("Is Basis" + basis.name + " linked ? " + basis.linked);
						} 
						else 
						{
							basis = animCube;
						}
						Debug.Log ("Basis : " + basis.name);
						if (basis.IsAgainstWall (-cubeNormal)) 
						{
							if (basis.transform.GetComponentInChildren<AnimCube>().IsAgainstWall (-cubeNormal)) 
							{
								basis = animCube;
								basis.transform.parent = GameObject.FindGameObjectWithTag("LevelContainer").transform;
							}

							basis.transform.GetComponentInChildren<AnimCube>().transform.parent = basis.transform.parent;
							basis = animCube.GetBasis ();
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

	void Pull (Vector3 dir)
	{
		ForceCheck();

		AnimCube basis;

		if (grabbedCube.linked == true) 
		{
			basis = grabbedCube.GetBasis ();
		} 
		else 
		{
			basis = grabbedCube;
		}

		if (basis.IsAgainstWall (dir)) 
		{
			if (basis.transform.GetComponentInChildren<AnimCube>().IsAgainstWall (dir)) 
			{
				basis = grabbedCube;
				basis.transform.parent = GameObject.FindGameObjectWithTag ("LevelContainer").transform;
			}

			basis.transform.GetComponentInChildren<AnimCube>().transform.parent = basis.transform.parent;
			basis = grabbedCube.GetBasis ();
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

	void PlaceGrapCollider(Vector3 targetPoint)
	{
		grappinCollider.enabled = true;
		Vector3 toCube = grabbedCube.transform.position - transform.position;
		grappinCollider.transform.position = transform.position + (toCube) / 2 - toCube.normalized * (grabbedCube.transform.GetComponent<BoxCollider>().size.x/4f);
		Vector3 newSize = new Vector3(4f,4f, (transform.position - targetPoint).magnitude * 3);


<<<<<<< HEAD:Assets/Scripts/3C/RobotController.cs
        Ray ray = new Ray (transform.position, transform.forward);
=======

        //Debug.Log("Attaching hook");
        Ray ray = new Ray (transform.position, transform.forward);

>>>>>>> 4f570ff25fa5cee53cdffd2a8159094ca94a2001:Assets/Scripts/RobotController.cs
		grappinCollider.size = newSize;
		grappinCollider.transform.forward = (grabbedCube.transform.position - grappinCollider.transform.position);
	}

	void AttachHook ()
	{
<<<<<<< HEAD:Assets/Scripts/3C/RobotController.cs
		Ray ray = new Ray(transform.position, transform.forward);
=======
		//Debug.Log("Attaching hook");
		Ray ray = new Ray(transform.position, transform.forward);

>>>>>>> 4f570ff25fa5cee53cdffd2a8159094ca94a2001:Assets/Scripts/RobotController.cs
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, pullRange, bumpForwardMask))
		{
			Vector3 cubeNormal = hit.normal;
			AnimCube animCube;
			/*if (Vector3.Dot(cubeNormal, hit.transform.forward) > 0.5 ||
				Vector3.Dot(cubeNormal, -hit.transform.forward) > 0.5 ||
				Vector3.Dot(cubeNormal, hit.transform.right) > 0.5 ||
				Vector3.Dot(cubeNormal, -hit.transform.right) > 0.5)*/

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
<<<<<<< HEAD:Assets/Scripts/3C/RobotController.cs
		if (input.bot.bump.WasReleased && IsGrounded()) 
=======
		
		if (Input.GetKeyUp(KeyCode.E) && IsGrounded())
>>>>>>> 4f570ff25fa5cee53cdffd2a8159094ca94a2001:Assets/Scripts/RobotController.cs
		{
			ForceCheck();

			GameObject particleImpact = Resources.Load("Particles/ImpactGround") as GameObject;
			particleImpact = Instantiate(particleImpact, transform.position - new Vector3(0, botHeight/2, 0), Quaternion.identity) as GameObject;
			particleImpact.transform.forward = Vector3.up;

			AnimCube animCube;

			//Getting every block in range, but not the ones below the player and bumping them according to the bumping force
			nearlyBlocks = Physics.OverlapSphere(transform.position, bumpRange, bumpUpMask);
			foreach (Collider col in nearlyBlocks)
			{
				//Debug.Log("Pos X Player : " + (transform.position.y - transform.localScale.y) + "Pos X Target : " + (col.transform.position.y - col.transform.localScale.y / 2));
				animCube = col.GetComponent<AnimCube>();
				if (col.transform.position.y - col.transform.localScale.y / 2 >= transform.position.y - botHeight/2 - 4f && animCube.bumping == false)
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

    public bool TryBumpKid() {
        GameObject kid = GameObject.Find("Kid");
        if (kid == null) { return false; } // Kid not found
        float distToKid = Vector3.Distance(transform.position, kid.transform.position);
        if (distToKid > bumpRange) { return false; } // Kid too far
        Abilities.JumpAbility jumpAbility = kid.GetComponentInChildren<Abilities.JumpAbility>();
        if (jumpAbility == null) { return false; } // Kid can't jump
        jumpAbility.ForceJumpRequest();
        return true; // Succesfully forced kid to jump
    }

    public override bool IsGrabbing() 
	{
        return isGrabbing;
    }

}
