using UnityEngine;
using System.Collections;

public class CrushBlocks : MonoBehaviour 
{
	public bool moved;
	public string particleName;

	void OnTriggerEnter(Collider other)
	{
		//Debug.Log ("Colliding !");
		if (other.transform.tag == "Robot" && moved == true) 
		{
			GameObject deathParticle = Instantiate (Resources.Load ("Particles/" + particleName) as GameObject, transform.position, Quaternion.identity) as GameObject;
			// Play Fmod sound here
			Destroy (gameObject);
		}
	}
}
