using UnityEngine;
using System.Collections;

public class LifeTime : MonoBehaviour 
{
	public float lifeTime;
	float timer;
	// Use this for initialization
	void Start () 
	{
		timer = lifeTime;
	}
	
	// Update is called once per frame
	void Update () 
	{
		timer -= Time.deltaTime;
		if (timer <= 0f) 
		{
			Destroy (gameObject);
		}
	}
}
