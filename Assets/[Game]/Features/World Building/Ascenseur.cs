using UnityEngine;
using System.Collections;

public class Ascenseur : MonoBehaviour {
	
	public float waitingTime;
	public float timeToMove;


	public IEnumerator  Ascend ( Vector3 endPos, Vector3 initialPosition)
	{
		
		float progress = 0f;

		while( progress < 1)
		{
			progress += Time.deltaTime / timeToMove;
			Vector3 position = transform.position;
			position.y = Mathf.Lerp(initialPosition.y, endPos.y, progress);
			transform.position = position;

			yield return null;
		
		}

		yield return new WaitForSeconds(waitingTime);
		progress = 0f;
		StartCoroutine(Ascend(initialPosition, endPos ));
	}
}
