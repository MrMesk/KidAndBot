using UnityEngine;
using System.Collections;

public class AnimCube : MonoBehaviour 
{
	[Header("Animation Curves")]
	public AnimationCurve moveUpCurve; // Used for upwards movement smoothing
	public AnimationCurve fallingCurve; // Used for downwards movement smoothing
	public AnimationCurve moveForwardCurve; // Used for directional movement smoothing

	[Header("Layer Masks")]
	public LayerMask bumpMask; // Used to check if there is a Level Module before the target destination
	public LayerMask rayMask; // Used to check if there is a Level Module below
	public LayerMask checkMask; // Used for obstacle checking when pushing a block

	[Header("Levitation Parameters")]
	public float levitationRadius; // Distance from which the Kid is considered close to the block
	public float levitationTime; // Levitation time if the kid isn't nearby

	[HideInInspector]public bool bumping; // Is the LM currently being pushed upwards ?
	[HideInInspector]public bool linked = false; //Is the LM Linked with another one ? (On top/Below it)

	[Header("SFX")]
	[FMODUnity.EventRef]
	public string click = "event:/Step";
	[FMODUnity.EventRef]
	public string move = "event:/Step";

	Transform kid;

	/// <summary>
	/// Used to determine the final position of the Level Module at the end of the bumping. Is updated dynamically with each interaction, to allow multiple interactions in a short amount of time.
	/// </summary>
	Vector3 bumpPos;
	float lerpState; // Set from reading Animation Curves and allows smooth movement
	float moveEvaluateIndex;// Used to evaluate Animation Curves
	int cubeScale;
	Vector3 initialPos;
	Vector3 movingPos;
	float bumpTimer;

	// Use this for initialization
	void Start () 
	{
		Init();
	}

	private void Init ()
	{
		kid = FindObjectOfType<Gameplay.KidCharacter>().transform;
		linked = false;
		bumpPos = transform.position;
		lerpState = 0f;
		moveEvaluateIndex = 0f;
		bumpTimer = 0f;
		bumping = false;

		cubeScale = Mathf.FloorToInt(GetComponent<BoxCollider>().size.x);

		LMCheckBelow();
	}

	#region DirectionalLevelModulePush
	/// <summary>
	/// Used to calculate bumpPos, depending on whether or not there are obstacles on the way and if there is ground below the available positions.
	/// </summary>
	/// <param name="maxDistance">Maximum distance at which the Level Module can go</param>
	/// <param name="dir">Bumping direction</param>
	/// <returns></returns>
	public Vector3 GetFarthestPoint (int maxDistance, Vector3 dir)
	{
		Vector3 farthestPoint = bumpPos;
		for (int i = cubeScale; i <= maxDistance; i += cubeScale)
		{
			//Debug.Log("i = " + i);
			Vector3 checkPos = bumpPos + dir.normalized * i;

			RaycastHit hit;
			//Debug.Log("Cube Scale : " + (cubeScale * 2 + 1));

			if (Physics.Raycast(checkPos, Vector3.down, out hit, cubeScale / 2 + 1))
			{
				Collider[] col = Physics.OverlapBox(checkPos, new Vector3(23.5f, 23.5f, 23.5f), Quaternion.identity, checkMask);

				if (col.Length == 0)
				{
					//Debug.Log("No clutter around check pos !");
					farthestPoint = checkPos;
				}
				else
				{
					Debug.Log("There are obstacles on the way !");
					foreach (Collider clutter in col)
					{
						Debug.Log("Obstacle name : " + clutter.name);
					}
					break;
				}
				//Debug.Log("Position " + checkPos + "Accessible !");
			}
			else
			{
				break;
			}
		}
		Debug.Log("Furthest position available in blocks : " + Vector3.Distance(farthestPoint,transform.position)/cubeScale);
		//Debug.Log("Farthest point : " + farthestPoint);
		return farthestPoint;
	}

	/// <summary>
	/// Calls GetFarthestPoint to get the bumpPos, and starts moving with BumpToDir Coroutine
	/// </summary>
	/// <param name="bumpForce"></param>
	/// <param name="bumpDir"></param>
	public void BumpingToDir(float bumpForce, Vector3 bumpDir)
	{
		//Debug.Log("Bumping to :" + bumpDir + " At " + bumpForce + " force ");
		RaycastHit hit;
		if (Physics.Raycast(transform.position, bumpDir, out hit, bumpForce, bumpMask))
		{
			//Debug.Log("Raycast hits !");
			//Debug.Log("Bump force " + (Vector3.Magnitude(transform.position - hit.point) - transform.GetComponent<BoxCollider>().size.x * 2));
			bumpPos = GetFarthestPoint(Mathf.RoundToInt(Vector3.Magnitude(transform.position - hit.point) - transform.GetComponent<BoxCollider>().size.x / 2), bumpDir);
		}
		else
		{
			//Debug.Log("All clear");
			//Debug.Log("Bump force " + bumpForce);
			bumpPos = GetFarthestPoint(Mathf.RoundToInt(bumpForce), bumpDir);
			//bumpPos += bumpDir * bumpForce;
		}
		FMODUnity.RuntimeManager.PlayOneShot(move, transform.position);
		StopCoroutine (BumpToDir (1f));
		StartCoroutine (BumpToDir (1f));
	}

	/// <summary>
	/// Moves bumped Level Module to bumpPos, which is calculated in BumpingToDir. Follows moveForwardCurve Animation Curve for smooth movement
	/// </summary>
	/// <param name="bumpTime">Time in seconds during which the Level Module will move towards its destination</param>
	/// <returns></returns>
	public IEnumerator BumpToDir(float bumpTime)
	{
		initialPos = transform.position;
		moveEvaluateIndex = 0f;
		lerpState = 0f;
		while (lerpState < bumpTime) 
		{
			moveEvaluateIndex += Time.deltaTime / bumpTime;
			lerpState = moveForwardCurve.Evaluate(moveEvaluateIndex);
			transform.position = Vector3.Lerp(initialPos, bumpPos, lerpState);
			yield return null;
		}

		lerpState = 0f;
		LMCheckBelow ();

	}

	#endregion

	#region LevelModuleBumpUpwards
	/// <summary>
	/// Activates the bumping mode when hit by Ground Slam. Follows the moveUpCurve Animation Curve, then levitates for a fixed amount of time
	/// </summary>
	/// <param name="bumpTime">Time in seconds during which the cube goes upwards</param>
	/// <param name="bumpForce">Used to determine desired height, depending on Ground slam strength</param>
	/// <returns></returns>
	public IEnumerator BumpUp(float bumpTime, float bumpForce)
	{
		bumping = true;
		initialPos = transform.position;
		movingPos = transform.position;
		while (bumpTimer < 1f) 
		{
			//Debug.Log ("Bump Timer : " + bumpTimer);
			bumpTimer += Time.deltaTime / bumpTime;
			movingPos.y = initialPos.y + moveUpCurve.Evaluate (bumpTimer) * bumpForce;
			transform.position = movingPos;
			yield return null;
		}

		bumpTimer = 0f;
		StartCoroutine(Levitating(bumpTime, bumpForce));
	}

	/// <summary>
	/// Levitates for a fixed amount of time that resets as long as the kid is near the block. Goes then into falling state
	/// </summary>
	/// <param name="bumpTime">Carrying previous BumpTime to calculate falling time</param>
	/// <param name="bumpForce">Carrying previous BumpForce to calculate landing position</param>
	/// <returns></returns>
	public IEnumerator Levitating(float bumpTime, float bumpForce)
	{
		bumpTimer = levitationTime;

		while(bumpTimer > 0f)
		{
			// If the Kid is nearby, resets the timer
			if(Vector3.Distance(kid.position, transform.position) < levitationRadius)
			{
				bumpTimer = levitationTime;
				Debug.Log("Kid is in Range !");
			}
			else
			{
				bumpTimer -= Time.deltaTime;
			}
			yield return null;
		}
		//Enter falling state. The values are used to normalize falling speed
		StartCoroutine(Falling((bumpTime / 384) * bumpForce, bumpForce));
	}

	/// <summary>
	/// Activates the falling State when Levitation state ends. Follows the fallingCurve Animation Curve
	/// </summary>
	/// <param name="fallTime">Time in seconds during which the block is going to fall</param>
	/// <param name="bumpForce">Used to calculate landing position</param>
	/// <returns></returns>
	public IEnumerator Falling(float fallTime, float bumpForce)
	{
		initialPos = transform.position;
		movingPos = transform.position;
		while (bumpTimer < 1f)
		{
			bumpTimer += Time.deltaTime / fallTime;
			movingPos.y = initialPos.y - fallingCurve.Evaluate(bumpTimer) * bumpForce;
			transform.position = movingPos;
			yield return null;
		}

		//Visual feedback
		GameObject particleImpact = Resources.Load("Particles/ImpactGround") as GameObject;
		particleImpact = Instantiate(particleImpact, transform.position - new Vector3(0, transform.GetComponent<BoxCollider>().size.y / 2, 0), Quaternion.identity) as GameObject;
		particleImpact.transform.forward = Vector3.up;
		//Add sound feedback here

		initialPos = transform.position;
		bumpTimer = 0f;
		bumping = false;
	}
	#endregion

	#region LevelModuleStacking

	/// <summary>
	/// Checks if a Level Module is below, and sets it as parent if there is one
	/// </summary>
	void LMCheckBelow ()
	{
		RaycastHit hit;
		if (Physics.Raycast (transform.position, Vector3.down, out hit, cubeScale / 2 + 1, rayMask)) 
		{
			//Debug.Log ("A Level Module is below !!!");
			linked = true;
			hit.transform.GetComponent<AnimCube> ().linked = true;
			FMODUnity.RuntimeManager.PlayOneShot(click, transform.position);
			transform.parent = hit.transform;
		}
	}
	
	/// <summary>
	/// Checks parents until we get to the lowest Level Module from the pile.
	/// </summary>
	/// <returns></returns>
	public AnimCube GetBasis()
	{
		Transform LMParent;
		LMParent = transform.parent;
		//Debug.Log ("LM Parent : " + LMParent.name);
		if (LMParent.tag == "LevelContainer") 
		{
			//Debug.Log ("This LM Is already at basis !");
			return GetComponent<AnimCube> ();
		}

		AnimCube below = LMParent.GetComponent<AnimCube> ();
		//Debug.Log ("Below : " + below.name);
		if (below != null) 
		{
			Transform bottomPoint = below.transform.parent;
			if (bottomPoint != null && bottomPoint.GetComponent<AnimCube>() != null) 
			{
				return bottomPoint.GetComponent<AnimCube> ();
			} 
			else 
			{
				return below.GetComponent<AnimCube> ();
			}
		} 
		else 
		{
			return GetComponent<AnimCube> ();
		}
	}

	public bool IsAgainstWall(Vector3 dir)
	{
		float rayLength = cubeScale / 2 + 1f;
		//Debug.Log ("Ray length : " + rayLength);
		if (Physics.Raycast (transform.position, dir, rayLength, bumpMask)) 
		{
			//Debug.Log (gameObject.name + " is against a wall !");
			return true;
		} 
		else 
		{
			//Debug.Log (gameObject.name + " isn't against a wall !");
			return false;
		}
	}
	#endregion
}
