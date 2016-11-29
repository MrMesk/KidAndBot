using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
	Camera cam;

	void Start ()
	{
		cam = GetComponent<Camera>();
	}
	public IEnumerator Shake (float shakeDuration, float magnitude)
	{

		float elapsed = 0.0f;

		Vector3 originalCamPos = Vector3.zero;

		while (elapsed < shakeDuration)
		{

			elapsed += Time.deltaTime;

			float percentComplete = elapsed / shakeDuration;
			float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

			// map value to [-1, 1]
			float x = Random.value * 2.0f - 1.0f;
			float y = Random.value * 2.0f - 1.0f;
			x *= magnitude * damper;
			y *= magnitude * damper;

			cam.transform.localPosition = new Vector3(x, y, originalCamPos.z);

			yield return null;
		}

		cam.transform.localPosition = originalCamPos;
	}
}
