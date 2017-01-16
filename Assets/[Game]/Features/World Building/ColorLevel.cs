using UnityEngine;
using System.Collections;

public class ColorLevel : MonoBehaviour {

	public Material level1;
	public Material level2;
	public Material level3;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(transform.position.y <= 12 ) 
		{
			this.gameObject.GetComponent<Renderer>().material = level1;
		}

		if(transform.position.y >= 12 && transform.position.y <= 24) 
		{
			this.gameObject.GetComponent<Renderer>().material = level2;
		}

		if(transform.position.y >= 24) 
		{
			this.gameObject.GetComponent<Renderer>().material = level3;
		}
	}
}
