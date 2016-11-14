using UnityEngine;
using System.Collections;

public class ActivateCrackEffect : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.C))
		{
			GameObject particleImpact2 = Resources.Load("Particles/ImpactGroundBump") as GameObject;
			//Debug.Log(particleImpact2);
			particleImpact2 = Instantiate (particleImpact2) as GameObject;
			//Debug.Log("Impact");
			GameObject particleImpact = Resources.Load("Particles/CrackEffect") as GameObject;
			//Debug.Log(particleImpact);
			particleImpact = Instantiate (particleImpact) as GameObject;

	


		
		}
	}
}
