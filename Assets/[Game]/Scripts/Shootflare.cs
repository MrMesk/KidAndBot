using UnityEngine;
using System.Collections;

public class Shootflare : MonoBehaviour
{
	public GameObject flare;
	public float shotSpeed;

	public GameObject kidCam;
	private Character character;
	private PlayerInput.Controller input { get { return character.input; } }
	// Use this for initialization
	void Start ()
	{
		character = GetComponentInParent<Character>();
		if (character == null)
		{
			this.enabled = false;
			return;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Input.GetButtonDown("Fire1"))
		{
			ShootFlare();
		}
	}

	void ShootFlare()
	{
		Vector3 dir = kidCam.transform.forward + Vector3.up;
		GameObject shot = Instantiate(flare, transform.position + dir.normalized / 5, Quaternion.identity) as GameObject;
		shot.GetComponent<Rigidbody>().AddForce(dir * shotSpeed);
	}
}
