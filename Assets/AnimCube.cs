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
	// Use this for initialization
	void Start () 
	{
		bumpTimer = 0f;
		bumping = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		/*
		if (Input.GetKeyDown (KeyCode.E) && bumping == false) 
		{
			bumping = true;
			StartCoroutine (BumpUp (2f, 10f));
		}
		*/
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
		initialPos = transform.position;
		bumpTimer = 0f;
		bumping = false;
	}
}
