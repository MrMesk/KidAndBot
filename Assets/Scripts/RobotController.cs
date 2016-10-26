using UnityEngine;
using System.Collections;

public class RobotController : MonoBehaviour 
{
	Collider[] nearlyBlocks;
	public float bumpRange;
	public float[] bumpTiers = new float[4];
	public float[] bumpForces = new float[4];
	public LayerMask bumpMask;

	float bumpForce;
	int forceTier;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKey (KeyCode.E)) 
		{
			bumpForce += Time.deltaTime;
			bumpForce = Mathf.Clamp (bumpForce, 0f, bumpTiers [3] + 0.1f);
		} 
		else if (Input.GetKeyUp (KeyCode.E)) 
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

			//Debug.Log ("Force bump : " + bumpForces [forceTier]);
			//Debug.Log ("Bump Force : " + bumpForce);

			AnimCube animCube;

			//Getting every block in range, but not the ones below the player and bumping them according to the bumping force
			nearlyBlocks = Physics.OverlapSphere (transform.position, bumpRange, bumpMask);
			foreach (Collider col in nearlyBlocks) 
			{
				animCube = col.GetComponent<AnimCube> ();
				if (col.transform.position.y >= transform.position.y && animCube.bumping == false) 
				{
					animCube.StartCoroutine (animCube.BumpUp (2f, bumpForces[forceTier]));
				}
			}
			bumpForce = 0f;
		}
	}
}
