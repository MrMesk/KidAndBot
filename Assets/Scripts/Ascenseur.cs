using UnityEngine;
using System.Collections;

public class Ascenseur : MonoBehaviour {

	public GameObject pointUp;
	public GameObject pointDown;
	public float speed;

	public bool goDown;
	public bool goUp;

	// Use this for initialization
	void Start () 
	{
		goUp = true;

	}
	
	// Update is called once per frame
	void Update () 


	{	// Monte


		if(transform.position.y >= 0  && goUp == true) 
		{
			//transform.position.y += pointUp.transform.position.y * speed ;
			transform.position += Vector3.up * speed * Time.deltaTime;
			if(transform.position.y > 11 ) 
			{
				goUp = false;
				goDown = true;

			}
		}

		//Descend
		if(transform.position.y <= 15 && goDown == true ) 
		{
			Debug.Log("Doit Descendre");
			//transform.position.y += pointDown.transform.position.y  * speed ;
			transform.position += Vector3.down * speed * Time.deltaTime;
			if(transform.position.y >= 0 ) 
			{
				goUp = true;
				goDown = false;

			}
		}


	}
}
