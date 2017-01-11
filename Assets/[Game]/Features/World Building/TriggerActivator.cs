using UnityEngine;
using System.Collections;

public class TriggerActivator : MonoBehaviour
{
	public MovingPlatform target;
	Vector3 targetInitialPos;
	public Vector3 targetPos;

	public bool oneTimeUse;
	public bool deactivateOnLeave;

	// Use this for initialization
	void Start ()
	{
		targetInitialPos = target.transform.position;
	}

	void OnTriggerEnter (Collider col)
	{
		if(target.isActivated == false)
		{
			target.isActivated = true;
			target.StartCoroutine(target.Activate(targetPos, targetInitialPos));

			if(oneTimeUse)
			{
				Destroy(gameObject);
			}
		}
	}

	private void OnTriggerExit (Collider other)
	{
		if(deactivateOnLeave)
		{
			target.StopCoroutine(target.Activate(targetPos, targetInitialPos));
			target.isActivated = false;
		}
	}

}