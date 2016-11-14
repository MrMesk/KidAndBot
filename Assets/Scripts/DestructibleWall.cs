using UnityEngine;
using System.Collections;

public class DestructibleWall : MonoBehaviour
{
	public float randomProjectionAngle;
	bool hasExploded = false;
	public void Destruct (Vector3 direction, float bumpForce)
	{
		if(hasExploded == false)
		{
			int childCount = transform.childCount;
			GameObject[] bricks = new GameObject[childCount];

			for (int i = 0; i < childCount; ++i)
			{
				Vector3 bumpDir = Quaternion.Euler(Random.Range(-randomProjectionAngle, randomProjectionAngle), Random.Range(-randomProjectionAngle, randomProjectionAngle), Random.Range(-randomProjectionAngle, randomProjectionAngle)) * direction * bumpForce;
				Rigidbody childRigid = transform.GetChild(i).GetComponent<Rigidbody>();
				CrushBlocks childCrush = transform.GetChild (i).GetComponent<CrushBlocks> ();
				childCrush.moved = true;
				childRigid.isKinematic = false;
				childRigid.AddForce(bumpDir);
			}

			GetComponent<BoxCollider>().enabled = false;
		}
		

	}

}
