using UnityEngine;
using System.Collections;

public class AnimCube : MonoBehaviour 
{
	public AnimationCurve moveUp;
	public AnimationCurve moveForward;
	Vector3 initialPos;
	Vector3 movingPos;
	float bumpTimer;
	public bool bumping;
	public LayerMask bumpMask;
	Vector3 bumpPos;
	float lerpState;
	float moveEvaluateIndex;

	// Use this for initialization
	void Start () 
	{
		bumpPos = transform.position;
		lerpState = 0f;
		moveEvaluateIndex = 0f;
		bumpTimer = 0f;
		bumping = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(transform.position != bumpPos)
		{
			transform.position = Vector3.Lerp(initialPos, bumpPos, lerpState);
		}
	}

	public void BumpingToDir(float bumpForce, Vector3 bumpDir)
	{
		Debug.Log("Bumping to :" + bumpDir + " At " + bumpForce + " force ");
		RaycastHit hit;
		if (Physics.Raycast(transform.position, bumpDir, out hit, bumpForce, bumpMask))
		{
			Debug.Log("Raycast hits !");
			bumpPos += bumpDir * (Vector3.Magnitude(transform.position - hit.point) - transform.localScale.x / 2);
		}
		else
		{
			Debug.Log("All clear");
			bumpPos += bumpDir * bumpForce;
		}
		StartCoroutine (BumpToDir (1f));
	}

	public IEnumerator BumpToDir(float bumpTime)
	{
		initialPos = transform.position;
		moveEvaluateIndex = 0f;
		lerpState = 0f;
		while (lerpState < bumpTime) 
		{
			moveEvaluateIndex += Time.deltaTime;
			lerpState = moveForward.Evaluate(moveEvaluateIndex/bumpTime);
			yield return null;
		}

		lerpState = 0f;

	}

	public IEnumerator BumpUp(float bumpTime, float bumpForce)
	{
		bumping = true;
		initialPos = transform.position;
		movingPos = transform.position;
		while (bumpTimer < 1f) 
		{
			//Debug.Log ("Bump Timer : " + bumpTimer);
			bumpTimer += Time.deltaTime / bumpTime;
			movingPos.y = initialPos.y + moveUp.Evaluate (bumpTimer) * bumpForce;
			transform.position = movingPos;
			yield return null;
		}

		GameObject particleImpact = Resources.Load("Particles/ImpactGround") as GameObject;
		particleImpact = Instantiate (particleImpact, transform.position - new Vector3(0, transform.localScale.y/2,0), Quaternion.identity) as GameObject;
		particleImpact.transform.forward = Vector3.up;

		initialPos = transform.position;
		bumpTimer = 0f;
		bumping = false;

	}
}
