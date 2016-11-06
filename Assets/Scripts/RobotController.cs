using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RobotController : Character 
{
	Collider[] nearlyBlocks;
	public float bumpRange;
	public float bumpForwardRange;
	public float pullRange;
	public float[] bumpTiers = new float[4];
	public float[] bumpForcesUp = new float[4];
	public float[] bumpForcesForward = new float[4];
	public LayerMask bumpUpMask;
	public LayerMask bumpForwardMask;

	

	float bumpForce;
	int forceTier;

	public Slider forceBar;

	bool isGrabbing;
	AnimCube grabbedCube;
	Vector3 grabbedNormal = new Vector3();
	public LineRenderer grappin;
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

	// Use this for initialization
	void Start () 
	{
		if(cameraShaker == null)
		{
			cameraShaker = _characterCamera.transform.Find("BotCam").GetComponent<CameraShake>();
		}
		grappin.enabled = false;
		if (forceBar == null) 
		{
			forceBar = GameObject.Find ("ForceBar").GetComponent<Slider> ();
		}
	}

	// Update is called once per frame
	protected override void FixedUpdate()
	{
		if (!isGrabbing) 
		{
			base.FixedUpdate ();
		}
	}
	protected override void Update () 
	{
		base.Update();

		ChargeManagement ();

		forceBar.value = Mathf.Lerp(forceBar.value, bumpForce, 0.1f);

		if (Input.GetMouseButtonDown (1)) 
		{
			Debug.Log ("Right Mouse Click");
			if (!isGrabbing) 
			{
				AttachHook ();
			} 
			else 
			{
				DetachHook ();
			}
		}
		if (isGrabbing) 
		{
			grappin.enabled = true;
			grappin.SetPosition (0, transform.position);
			grappin.SetPosition (1, grabbedCube.transform.position);

			if (Input.GetKeyDown (KeyCode.A)) 
			{
				Pull (grabbedNormal);
				isGrabbing = false;
				grabbedCube = null;
				grabbedNormal = new Vector3();
			}
		} 
		else 
		{
			BumpUp ();
			BumpForward ();
			grappin.enabled = false;
		}

		if (_directionalInput != Vector2.zero && mobilityState == MobilityState.GROUNDED)
		{
			WalkingShake();
		}

		if (Input.GetButtonDown("Jump"))
		{
			cameraShaker.StartCoroutine(cameraShaker.Shake(walkShakeDuration*2, walkShakeMagnitude*2));
			GameObject particleImpact = Resources.Load("Particles/ImpactJump") as GameObject;
			particleImpact = Instantiate(particleImpact, transform.position - new Vector3(0, transform.localScale.y / 2, 0), Quaternion.identity) as GameObject;
			particleImpact.transform.forward = Vector3.up;
		}
	}

	void WalkingShake()
	{
		walkTimer += Time.deltaTime;
		if(walkTimer >= walkShakeFrequency)
		{
			walkTimer = 0f;
			cameraShaker.StartCoroutine(cameraShaker.Shake(walkShakeDuration, walkShakeMagnitude));
		}
	}
	void ChargeManagement()
	{
		
		if (Input.GetKey (KeyCode.LeftShift)) 
		{
			Debug.Log ("Charging");
			bumpForce += Time.deltaTime;
			bumpForce = Mathf.Clamp (bumpForce, 0f, bumpTiers [3] * 1.25f);
		} 
		else 
		{
			bumpForce -= Time.deltaTime;
			bumpForce = Mathf.Clamp (bumpForce, 0f, bumpTiers [3] * 1.25f);
		}
	}

	void ForceCheck()
	{
		if (bumpForce >= bumpTiers [3]) 
		{
			forceTier = 3;
		}
		else if (bumpForce >= bumpTiers [2]) 
		{
			forceTier = 2;
		}
		else if (bumpForce >= bumpTiers [1]) 
		{
			forceTier = 1;
		} 
		else 
		{
			forceTier = 0;
		}
		Debug.Log ("Force Tier : " + forceTier);
		bumpForce = 0f;
	}

	void BumpForward()
	{
		if (Input.GetMouseButtonDown(0)) 
		{
			Ray ray = new Ray (transform.position, transform.forward);
			RaycastHit hit;

			ForceCheck ();

			Debug.DrawRay (ray.origin, ray.direction * 20f, Color.red);
			if (Physics.Raycast (ray, out hit, bumpForwardRange,bumpForwardMask )) 
			{
				//Debug.Log ("Raycast hits !!");
				Vector3 cubeNormal = hit.normal;
				AnimCube animCube;
				if (Vector3.Dot (cubeNormal, hit.transform.forward) > 0.5f ||
					Vector3.Dot (cubeNormal, -hit.transform.forward) > 0.5f ||
					Vector3.Dot (cubeNormal, hit.transform.right) > 0.5f ||
					Vector3.Dot (cubeNormal, -hit.transform.right) > 0.5f) 
				{
					animCube = hit.transform.GetComponent<AnimCube> ();
					if (animCube.bumping == false) 
					{
						GameObject particleImpact = Resources.Load("Particles/Impact"+forceTier) as GameObject;
						particleImpact = Instantiate (particleImpact, hit.point, Quaternion.identity) as GameObject;
						particleImpact.transform.forward = cubeNormal;
						//animCube.StartCoroutine (animCube.BumpToDir (2f, bumpForcesForward [forceTier], -cubeNormal));
						animCube.BumpingToDir(bumpForcesForward[forceTier], -cubeNormal);
					}


				}
				cameraShaker.StartCoroutine(cameraShaker.Shake(shakeTiersDuration[forceTier], shakeTiersMagnitude[forceTier]));
			} 
		}
	}

	void Pull(Vector3 dir)
	{
		ForceCheck ();
		//grabbedCube.StartCoroutine (grabbedCube.BumpToDir (2f, bumpForcesForward [forceTier], dir));
		grabbedCube.BumpingToDir(bumpForcesForward[forceTier], dir);
		cameraShaker.StartCoroutine(cameraShaker.Shake(shakeTiersDuration[forceTier], shakeTiersMagnitude[forceTier]));
	}

	void DetachHook()
	{
		grabbedCube = null;
		isGrabbing = false;
	}

	void AttachHook()
	{
		bumpForce = 0f;

		Debug.Log ("Attaching hook");
		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, pullRange)) 
		{
			//Debug.Log ("Raycast hits !!");
			Vector3 cubeNormal = hit.normal;
			AnimCube animCube;
			if (Vector3.Dot (cubeNormal, hit.transform.forward) > 0 ||
				Vector3.Dot (cubeNormal, -hit.transform.forward) > 0 ||
				Vector3.Dot (cubeNormal, hit.transform.right) > 0 ||
				Vector3.Dot (cubeNormal, -hit.transform.right) > 0) 
			{
				animCube = hit.transform.GetComponent<AnimCube> ();
				if (animCube.bumping == false) 
				{
					grabbedNormal = cubeNormal;
					grabbedCube = animCube;
					isGrabbing = true;
				}
			}
		} 
	}

	void BumpUp()
	{
		if (Input.GetKeyDown (KeyCode.E) && mobilityState == MobilityState.GROUNDED) 
		{
			ForceCheck ();

			GameObject particleImpact = Resources.Load("Particles/ImpactGround") as GameObject;
			particleImpact = Instantiate (particleImpact, transform.position - new Vector3(0, transform.localScale.y/2,0), Quaternion.identity) as GameObject;
			particleImpact.transform.forward = Vector3.up;

			AnimCube animCube;

			//Getting every block in range, but not the ones below the player and bumping them according to the bumping force
			nearlyBlocks = Physics.OverlapSphere (transform.position, bumpRange, bumpUpMask);
			foreach (Collider col in nearlyBlocks) 
			{
				Debug.Log ("Pos X Player : " + (transform.position.y - transform.localScale.y) + "Pos X Target : " + (col.transform.position.y - col.transform.localScale.y/2));
				animCube = col.GetComponent<AnimCube> ();
				if (col.transform.position.y - col.transform.localScale.y/2 >= transform.position.y - transform.localScale.y -1f && animCube.bumping == false) 
				{
					animCube.StartCoroutine (animCube.BumpUp (2f, bumpForcesUp[forceTier]));
				}
			}
			cameraShaker.StartCoroutine(cameraShaker.Shake(shakeTiersDuration[forceTier], shakeTiersMagnitude[forceTier]));

		}
	}


}
