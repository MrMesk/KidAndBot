using UnityEngine;
using System.Collections;

public class DestructibleWall : MonoBehaviour
{
	bool hasExploded = false;
	public void Destruct (Vector3 direction, float bumpForce)
	{
		if(hasExploded == false)
		{
			int childCount = transform.childCount;
			GameObject[] bricks = new GameObject[childCount];

			for (int i = 0; i < childCount; ++i)
			{
				Vector3 bumpDir = Quaternion.Euler(Random.Range(-90f, 90f), Random.Range(-90f, 90f), Random.Range(-90f, 90f)) * direction * bumpForce;
				Rigidbody childRigid = transform.GetChild(i).GetComponent<Rigidbody>();
				childRigid.isKinematic = false;
				childRigid.AddForce(bumpDir);
			}

			GetComponent<BoxCollider>().enabled = false;
		}
		

	}

}
