using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LMSnapping))]
public class LMSnappingEditor : Editor
{
	GameObject selectedGameObject = null;

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();
		LMSnapping lmSnap = (LMSnapping)target;

		GUILayout.Space(20);

		GUILayout.Label("Building options");

		GUILayout.Space(20);

		// All button-based building options

		// Instantiates a dynamic Level Module at cursor position and selects it
		if (GUILayout.Button("Build Level Module"))
		{
			GameObject lm = lmSnap.dynamicLevelModule;
			selectedGameObject = lmSnap.CreateLevelModule(lm);
			lmSnap.selectedLevelModule = selectedGameObject;
		}
		// Instantiates a static Level Module at cursor position and selects it
		if (GUILayout.Button("Build Static Level Module"))
		{
			GameObject lm = lmSnap.staticLevelModule;
			selectedGameObject = lmSnap.CreateLevelModule(lm);
			lmSnap.selectedLevelModule = selectedGameObject;
		}
		// Moves selected Level Module to cursor position
		if (GUILayout.Button("Move Selected Module"))
		{
			if (selectedGameObject != null)
			{
				selectedGameObject.transform.position = lmSnap.cursorPos;
				lmSnap.selectedLevelModule = selectedGameObject;
			}
			else
				Debug.Log("No object selected !");
		}
		// Selects the Level Module at cursor position
		if (GUILayout.Button("Select Level Module"))
		{
			selectedGameObject = lmSnap.SelectAtPoint();
			lmSnap.selectedLevelModule = selectedGameObject;
		}
		// Deletes the Level Module at cursor position
		if (GUILayout.Button("Delete Level Module"))
		{
			lmSnap.DeleteAtPoint();
		}

		GUILayout.Space(20);
		GUILayout.Label("Moving Cursor Position");
		GUILayout.Space(20);

		// All button-based Cursor movement options

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Down"))
		{
			lmSnap.MoveGizmo(Vector3.down);
			SceneView.RepaintAll();
		}
		if (GUILayout.Button("Forward"))
		{
			lmSnap.MoveGizmo(Vector3.forward);
			SceneView.RepaintAll();
		}
		if (GUILayout.Button("Up"))
		{
			lmSnap.MoveGizmo(Vector3.up);
			SceneView.RepaintAll();
		}
		GUILayout.EndHorizontal();
	
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Left"))
		{
			lmSnap.MoveGizmo(Vector3.left);
			SceneView.RepaintAll();
		}
		if (GUILayout.Button("Back"))
		{
			lmSnap.MoveGizmo(Vector3.back);
			SceneView.RepaintAll();
		}
		if (GUILayout.Button("Right"))
		{
			lmSnap.MoveGizmo(Vector3.right);
			SceneView.RepaintAll();
		}
		GUILayout.EndHorizontal();

		GUILayout.Space(20);

		// Resets cursor position to selected object
		if (GUILayout.Button("Reset cursor to selected object"))
		{
			if (selectedGameObject != null)
			{
				lmSnap.cursorPos = selectedGameObject.transform.position;

				if (lmSnap.centerViewOnCursor == true)
				{
					MoveSceneView();
				}

				SceneView.RepaintAll();
			}
			else
				Debug.Log("No object selected !");
		}

		GUILayout.Space(20);

		if (GUILayout.Button("Reset cursor to origin"))
		{
			lmSnap.cursorPos = new Vector3(0f,lmSnap.snapValue/2,0f);

			if (lmSnap.centerViewOnCursor == true)
			{
				MoveSceneView();
			}

			SceneView.RepaintAll();
		}

	}

	// Moves scene view pivot to cursor position
	void MoveSceneView()
	{
		LMSnapping lmSnap = (LMSnapping)target;
		Vector3 position = SceneView.lastActiveSceneView.pivot;
		position = lmSnap.cursorPos;
		SceneView.lastActiveSceneView.pivot = position;
		SceneView.lastActiveSceneView.Repaint();
	}

	private void OnSceneGUI ()
	{
		LMSnapping lmSnap = (LMSnapping)target;

		Event e = Event.current;
		switch (e.type)
		{
			case EventType.KeyDown:
			{
				if (Event.current.keyCode == (KeyCode.K))
				{
					lmSnap.MoveGizmo(Vector3.left);

					if(lmSnap.centerViewOnCursor == true)
					{
						MoveSceneView();
					}
					
					SceneView.RepaintAll();
				}
				else if (Event.current.keyCode == (KeyCode.M))
				{
					lmSnap.MoveGizmo(Vector3.right);

					if (lmSnap.centerViewOnCursor == true)
					{
						MoveSceneView();
					}

					SceneView.RepaintAll();
				}
				else if (Event.current.keyCode == (KeyCode.O))
				{
					lmSnap.MoveGizmo(Vector3.forward);

					if (lmSnap.centerViewOnCursor == true)
					{
						MoveSceneView();
					}

					SceneView.RepaintAll();
				}
				else if (Event.current.keyCode == (KeyCode.L))
				{
					lmSnap.MoveGizmo(Vector3.back);

					if (lmSnap.centerViewOnCursor == true)
					{
						MoveSceneView();
					}

					SceneView.RepaintAll();
				}
				else if (Event.current.keyCode == (KeyCode.P))
				{
					lmSnap.MoveGizmo(Vector3.up);

					if (lmSnap.centerViewOnCursor == true)
					{
						MoveSceneView();
					}

					SceneView.RepaintAll();
				}
				else if (Event.current.keyCode == (KeyCode.I))
				{
					lmSnap.MoveGizmo(Vector3.down);

					if (lmSnap.centerViewOnCursor == true)
					{
						MoveSceneView();
					}

					SceneView.RepaintAll();
				}
				else if (Event.current.keyCode == (KeyCode.F1))
				{
					GameObject lm = lmSnap.dynamicLevelModule;
					selectedGameObject = lmSnap.CreateLevelModule(lm);
					lmSnap.selectedLevelModule = selectedGameObject;
				}
				else if (Event.current.keyCode == (KeyCode.F2))
				{
					GameObject lm = lmSnap.staticLevelModule;
					selectedGameObject = lmSnap.CreateLevelModule(lm);
					lmSnap.selectedLevelModule = selectedGameObject;
				}
				else if (Event.current.keyCode == (KeyCode.F3))
				{
					if (selectedGameObject != null)
					{
						selectedGameObject.transform.position = lmSnap.cursorPos;
						lmSnap.selectedLevelModule = selectedGameObject;
					}
					else
						Debug.Log("No object selected !");
				}
				else if (Event.current.keyCode == (KeyCode.F4))
				{
					selectedGameObject = lmSnap.SelectAtPoint();
					lmSnap.selectedLevelModule = selectedGameObject;
				}
				else if (Event.current.keyCode == (KeyCode.F5))
				{
					lmSnap.DeleteAtPoint();
				}
				else if (Event.current.keyCode == (KeyCode.F7))
				{
					if (selectedGameObject != null)
					{
						lmSnap.cursorPos = selectedGameObject.transform.position;

						if (lmSnap.centerViewOnCursor == true)
						{
							MoveSceneView();
						}

						SceneView.RepaintAll();
					}
					else
						Debug.Log("No object selected !");
				}
				else if(Event.current.keyCode == (KeyCode.F8))
				{
					lmSnap.cursorPos = new Vector3(0f, lmSnap.snapValue / 2, 0f);

					if (lmSnap.centerViewOnCursor == true)
					{
						MoveSceneView();
					}

					SceneView.RepaintAll();
				}
				
				break;	
			}
		}
	}
}
