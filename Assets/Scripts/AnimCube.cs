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

	// Use this for initialization
	void Start () 
	{
		bumpPos = transform.position;

		bumpTimer = 0f;
		bumping = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(transform.position != bumpPos)
		{
			transform.position = Vector3.MoveTowards(transform.position, bumpPos, 0.3f);
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
	}

	public IEnumerator BumpToDir(float bumpTime, float bumpForce, Vector3 bumpDir)
	{
		bumping = true;
		initialPos = transform.position;
		movingPos = transform.position;

		//Debug.Log ("Dot : " + Vector3.Dot (bumpDir, transform.forward));
		if ( Vector3.Dot(bumpDir, transform.forward) > 0) 
		{
			while (bumpTimer < 1f) 
			{
				//Debug.Log ("Bump Timer : " + bumpTimer);
				bumpTimer += Time.deltaTime / bumpTime;
				movingPos.z = initialPos.z + moveForward.Evaluate (bumpTimer) * bumpForce;
				transform.position = movingPos;
				yield return null;
			}
		}
		else if ( Vector3.Dot(bumpDir, -transform.forward) > 0) 
		{
			while (bumpTimer < 1f) 
			{
				//Debug.Log ("Bump Timer : " + bumpTimer);
				bumpTimer += Time.deltaTime / bumpTime;
				movingPos.z = initialPos.z - moveForward.Evaluate (bumpTimer) * bumpForce;
				transform.position = movingPos;
				yield return null;
			}
		}
		else if ( Vector3.Dot(bumpDir, transform.right) > 0) 
		{
			while (bumpTimer < 1f) 
			{
				//Debug.Log ("Bump Timer : " + bumpTimer);
				bumpTimer += Time.deltaTime / bumpTime;
				movingPos.x = initialPos.x + moveForward.Evaluate (bumpTimer) * bumpForce;
				transform.position = movingPos;
				yield return null;
			}
		}
		else if ( Vector3.Dot(bumpDir, -transform.right) > 0) 
		{
			while (bumpTimer < 1f) 
			{
				//Debug.Log ("Bump Timer : " + bumpTimer);
				bumpTimer += Time.deltaTime / bumpTime;
				movingPos.x = initialPos.x - moveForward.Evaluate (bumpTimer) * bumpForce;
				transform.position = movingPos;
				yield return null;
			}
		}

		initialPos = transform.position;
		bumpTimer = 0f;
		bumping = false;
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
		particleImpact = Instantiate (particleImpact, transform.position - new Vector3(0, transform.localScale.y,0), Quaternion.identity) as GameObject;
		particleImpact.transform.forward = Vector3.up;

		initialPos = transform.position;
		bumpTimer = 0f;
		bumping = false;

	}
}
