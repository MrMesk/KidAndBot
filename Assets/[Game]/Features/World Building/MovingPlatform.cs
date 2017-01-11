using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
	
	public float idleTime = 5f;
	public float travelTime = 3f;

	public bool isActivated = false;
	float progress = 0f;

	public IEnumerator Activate (Vector3 endPos, Vector3 initialPosition)
	{
		while (progress < 1)
		{
			progress += Time.deltaTime / travelTime;
			Vector3 position = transform.position;
			position.y = Mathf.Lerp(initialPosition.y, endPos.y, progress);
			position.x = Mathf.Lerp(initialPosition.x, endPos.x, progress);
			position.z = Mathf.Lerp(initialPosition.z, endPos.z, progress);
			transform.position = position;
			yield return null;
		}

		yield return new WaitForSeconds(idleTime);
		progress = 0f;
		StartCoroutine(Activate(initialPosition, endPos));
	}
}
