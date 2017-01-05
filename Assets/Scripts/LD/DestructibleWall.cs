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
				childRigid.isKinematic = false;
				childRigid.AddForce(bumpDir * 50f);
				StartCoroutine(BlockInit(childCrush));
			}

			GetComponent<BoxCollider>().enabled = false;
		}
		

	}
	public IEnumerator BlockInit (CrushBlocks block)
	{
		yield return new WaitForSeconds(0.3f);
		if(block != null)
		{
			block.moved = true;
		}

	}
}


