using UnityEngine;
using System.Collections;

public class RobotController : Character 
{
	Collider[] nearlyBlocks;
	public float bumpRange;
	public float bumpForwardRange;
	public float pullRange;
	public float[] bumpTiers = new float[4];
	public float[] bumpForcesUp = new float[4];
	public float[] bumpForcesForward = new float[4];
	public LayerMask bumpMask;

	float bumpForce;
	int forceTier;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame

	protected override void Update () 
	{
		base.Update();
		BumpCheck ();
		BumpForward ();
	}

	void ChargeBump()
	{
		bumpForce += Time.deltaTime;
		bumpForce = Mathf.Clamp (bumpForce, 0f, bumpTiers [3] + 0.1f);

		//Debug.Log ("Bump Force : " + bumpForce);
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
	}

	void BumpForward()
	{
		//Debug.DrawRay (transform.position, directionalInput, Color.red);

		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;

		Debug.DrawRay (ray.origin, ray.direction * 20f, Color.red);

		if (Input.GetMouseButton(0)) 
		{
			ChargeBump ();
		} 
		if (Input.GetMouseButtonUp(0)) 
		{
			ForceCheck ();

			Debug.DrawRay (ray.origin, ray.direction * 20f, Color.red);
			if (Physics.Raycast (ray, out hit, bumpForwardRange)) 
			{
				Debug.Log ("Raycast hits !!");
				Vector3 cubeNormal = hit.normal;
				AnimCube animCube;
				//cubeNormal = hit.transform.TransformDirection (cubeNormal);

				if (Vector3.Dot (cubeNormal, hit.transform.forward) > 0 ||
				    Vector3.Dot (cubeNormal, -hit.transform.forward) > 0 ||
				    Vector3.Dot (cubeNormal, hit.transform.right) > 0 ||
				    Vector3.Dot (cubeNormal, -hit.transform.right) > 0) 
				{
					animCube = hit.transform.GetComponent<AnimCube> ();
					if (animCube.bumping == false) 
					{
						animCube.StartCoroutine (animCube.BumpToDir (2f, bumpForcesUp [forceTier], -cubeNormal));
					}
				}
			} 
			else 
			{
				bumpForce = 0f;
			}
		}

	}


	void BumpCheck()
	{
		//Charging the bump
		if (Input.GetKey (KeyCode.E)) 
		{
			ChargeBump ();
		} 
		else if (Input.GetKeyUp (KeyCode.E)) 
		{

			ForceCheck ();

			//Debug.Log ("Force bump : " + bumpForcesUp [forceTier]);
			//Debug.Log ("Bump Force : " + bumpForce);

			AnimCube animCube;

			//Getting every block in range, but not the ones below the player and bumping them according to the bumping force
			nearlyBlocks = Physics.OverlapSphere (transform.position, bumpRange, bumpMask);
			foreach (Collider col in nearlyBlocks) 
			{
				animCube = col.GetComponent<AnimCube> ();
				if (col.transform.position.y >= transform.position.y && animCube.bumping == false) 
				{
					animCube.StartCoroutine (animCube.BumpUp (2f, bumpForcesUp[forceTier]));
				}
			}
			bumpForce = 0f;
		}
	}


}
