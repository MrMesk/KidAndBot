using UnityEngine;
using System.Collections;

// Allows to create, select, move or delete static and dynamic Level Modules according to a custom cursor position perfectly snapped according to a fixed value.
// Displays the cursor in different colors depending on if the Build-O-Tron is selected or not. Also draws an outline over the selected Level Module.

public class LMSnapping : MonoBehaviour
{
	[Space(10)]
	[Header("Global parameters")]
	[Space(10)]
	
	public GameObject selectedLevelModule;

	[Space(10)]
	[Range(4, 48)]
	public int snapValue; // This value is used for moving the cursor in the world, + determining its size

	[Space(10)]

	public Transform lmParent; // Parent we are going to store our Level Modules in. This one can be changed for better sorting

	[Space(10)]

	public Vector3 cursorPos; // Position of the cursor in the world

	[Space(10)]

	public LayerMask deleteMask; // Layer Mask used for Deleting and Selecting Level Modules

	[Space(10)]

	public bool centerViewOnCursor = true; // Do we want the editor view pivot to stick to our cursor when we move it ?

	[Space(10)]

	[Header("Level Modules")]

	public GameObject staticLevelModule;
	public GameObject dynamicLevelModule;

	//Creates a Level module on cursor position
	public GameObject CreateLevelModule(GameObject levelModule)
	{
		DeleteAtPoint();

		GameObject lm;
		lm = Instantiate(levelModule, cursorPos, Quaternion.identity) as GameObject;
		lm.transform.parent = lmParent;
		return lm;
	}

	//Moves cursor depending on snap value and direction
	public void MoveGizmo(Vector3 dir)
	{
		cursorPos += dir * snapValue;
	}

	//Deletes Level Module at cursor coordinates
	public void DeleteAtPoint()
	{
		Collider[] toDelete;
		toDelete = Physics.OverlapSphere(cursorPos, snapValue / 4, deleteMask);
		if (toDelete != null)
		{
			foreach(Collider col in toDelete)
			{
				DestroyImmediate(col.gameObject);
			}
		}
	}

	//Returns Level Module at cursor coordinates
	public GameObject SelectAtPoint()
	{
		GameObject selectedLevelModule = null;
		Collider[] toSelect;
		toSelect = Physics.OverlapSphere(cursorPos, snapValue / 4, deleteMask);
		if (toSelect != null)
		{
			foreach (Collider col in toSelect)
			{
				selectedLevelModule = col.gameObject;
			}
		}
		return selectedLevelModule;
	}

	//Drawing the cursor
	private void OnDrawGizmos ()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(cursorPos, new Vector3(snapValue, snapValue, snapValue));

		Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
		Gizmos.DrawCube(cursorPos, new Vector3(snapValue, snapValue, snapValue));

		if(selectedLevelModule != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(selectedLevelModule.transform.position, new Vector3(snapValue+0.1f, snapValue+0.1f, snapValue+0.1f));
		}
	}

	private void OnDrawGizmosSelected ()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(cursorPos, new Vector3(snapValue, snapValue, snapValue));

		Gizmos.color = new Color(0f, 0f, 1f, 0.8f);
		Gizmos.DrawCube(cursorPos, new Vector3(snapValue, snapValue, snapValue));
	}
}
