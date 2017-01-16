using UnityEngine;
using System.Collections;
using InControl;

public class CameraController : MonoBehaviour
{
	public Transform target;

	[System.Serializable]
	public class PositionSettings
	{
		public Vector3 targetPosOffset = new Vector3(0, 3.4f, 0);
		public float lookSmooth = 100f;
		public float distanceFromTarget = -8;
		public float zoomSmooth = 10f;
		public float maxZoom = -2;
		public float minZoon = -15;
		public bool smoothFollow = true;
		public float smooth = 0.05f;

		[HideInInspector]
		public float newDistance = -8;
		[HideInInspector]
		public float adjustmentDistance = -8;

	}

	[System.Serializable]
	public class OrbitSettings
	{
		public float xRotation = -20f;
		public float yRotation = -180f;
		public bool invertXAxis = true;
		public bool invertYAxis = true;
		public float minXRotation = -85;
		public float maxXRotation = 25;
		public float vOrbitSmooth = 1f;
		public float hOrbitSmooth = 1f;
		public float resetCamTime = 2f;
	}

	[System.Serializable]
	public class InputSettings
	{
		public string ORBIT_HORIZONTAL_SNAP = "OrbitHorizontalSnap";
		public string ZOOM = "Mouse ScrollWheel";
	}

	[System.Serializable]
	public class DebugSettings
	{
		public bool drawDesiredCollisionLines = true;
		public bool drawAdjustedCollisionLines = true;
	}

	public PositionSettings position = new PositionSettings();
	public OrbitSettings orbit = new OrbitSettings();
	public InputSettings input = new InputSettings();
	public DebugSettings debug = new DebugSettings();
	public CollisionHandler coll = new CollisionHandler();

	Vector3 targetPos = Vector3.zero;
	Vector3 destination = Vector3.zero;
	Vector3 adjustedDestinaton = Vector3.zero;
	Vector3 camVel = Vector3.zero;

	Character character;
	float vOrbitInput, hOrbitInput, zoomInput, hOrbitSnapInput;
	Vector3 previousMousePos = Vector3.zero;
	Vector3 currentMousePos = Vector3.zero;

	float resetCamTimer;
	private void Start ()
	{
		resetCamTimer = orbit.resetCamTime;
		SetCameraTarget(target);

		MoveToTarget();

		coll.Initialize(Camera.main);
		coll.UpdateCameraClipPoints(transform.position, transform.rotation, ref coll.adjustedCameraClipPoints);
		coll.UpdateCameraClipPoints(destination, transform.rotation, ref coll.desiredCameraClipPoints);

		previousMousePos = currentMousePos = Input.mousePosition;

	}

	private void Update ()
	{
		//GetInput();
		ZoomInOnTarget();
	}

	void SetCameraTarget(Transform t)
	{
		target = t;

		if(target != null)
		{
			if(target.GetComponent<Character>())
			{
				character = target.GetComponent<Character>();
			}
			else
			{
				//Debug.Log("Target needs a character controller !");
			}
		}
		else
		{
			//Debug.Log("The camera doesn't have a target !");
		}
	}

	private void FixedUpdate ()
	{
		MoveToTarget();
		LookAtTarget();
		MouseOrbitTarget();

		coll.UpdateCameraClipPoints(transform.position, transform.rotation, ref coll.adjustedCameraClipPoints);
		coll.UpdateCameraClipPoints(destination, transform.rotation, ref coll.desiredCameraClipPoints);

		for(int i = 0; i<5; i++)
		{
			if(debug.drawDesiredCollisionLines)
			{
				Debug.DrawLine(targetPos, coll.desiredCameraClipPoints[i], Color.white);
			}
			if(debug.drawAdjustedCollisionLines)
			{
				Debug.DrawLine(targetPos, coll.adjustedCameraClipPoints[i], Color.green);
			}
		}

		coll.CheckColliding(targetPos); //using raycasts
		position.adjustmentDistance = coll.GetAdjustedDistanceWithRayFrom(targetPos);
	}

	void MouseOrbitTarget()
	{
		currentMousePos = character.input.shared.camera.Value;
		currentMousePos.x = orbit.invertXAxis ? -currentMousePos.x : currentMousePos.x;
		currentMousePos.y = orbit.invertYAxis ? -currentMousePos.y : currentMousePos.y;

		//Debug.Log ("Current Mouse Pos : " + currentMousePos);

		const float dead = 0f;
		if(currentMousePos.magnitude <= dead)
		{
			resetCamTimer -= Time.deltaTime;
			resetCamTimer = Mathf.Clamp(resetCamTimer, 0f, orbit.resetCamTime);
			//Debug.Log("reset cam timer : " + resetCamTimer);
		}
		//Possibly add snapping bool here to disable mouse orbit
		else
		{
			resetCamTimer = orbit.resetCamTime;
			orbit.yRotation += (currentMousePos.x) * orbit.vOrbitSmooth;
			orbit.xRotation += (currentMousePos.y) * orbit.hOrbitSmooth;
		}

		if(resetCamTimer == 0f)
		{
			orbit.yRotation = -180;
		}

		orbit.xRotation = Mathf.Clamp(orbit.xRotation, orbit.minXRotation, orbit.maxXRotation);
	}

	void ZoomInOnTarget()
	{
		position.distanceFromTarget += zoomInput * position.zoomSmooth * Time.deltaTime;

		position.distanceFromTarget = Mathf.Clamp(position.distanceFromTarget, position.minZoon, position.maxZoom);
	}

	void MoveToTarget()
	{
		targetPos = target.position + position.targetPosOffset;
		destination = Quaternion.Euler(orbit.xRotation, orbit.yRotation + target.eulerAngles.y, 0) * - Vector3.forward * position.distanceFromTarget;
		destination += target.position;

		//Add Smooth movement here
		if(coll.colliding)
		{
			adjustedDestinaton = Quaternion.Euler(orbit.xRotation, orbit.yRotation + target.eulerAngles.y, 0) * Vector3.forward * position.adjustmentDistance;
			adjustedDestinaton += targetPos;

			if(position.smoothFollow)
			{
				transform.position = Vector3.SmoothDamp(transform.position, adjustedDestinaton, ref camVel, position.smooth);
			}
			else
			{
				transform.position = adjustedDestinaton;
			}
		}
		else
		{
			if (position.smoothFollow)
			{
				transform.position = Vector3.SmoothDamp(transform.position, destination, ref camVel, position.smooth);
			}
			else
			{
				transform.position = destination;
			}
		}
	}

	void LookAtTarget ()
	{
		Quaternion targetRotation = Quaternion.LookRotation(targetPos - transform.position);
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, position.lookSmooth * Time.deltaTime);
	}

	[System.Serializable]
	public class CollisionHandler
	{
		public LayerMask collisionLayer;
		public float cushionSize = 3.41f;

		[HideInInspector]
		public bool colliding = false;
		[HideInInspector]
		public Vector3[] adjustedCameraClipPoints;
		[HideInInspector]
		public Vector3[] desiredCameraClipPoints;
		
		Camera myCam;

		public void Initialize(Camera cam)
		{
			myCam = cam;
			adjustedCameraClipPoints = new Vector3[5];
			desiredCameraClipPoints = new Vector3[5];
		}

		public void UpdateCameraClipPoints (Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] intoArray)
		{
			if (!myCam)
			{
				Debug.Log("No camera !!");
				return;
			}

			// Clearing the contents of intoArray
			intoArray = new Vector3[5];

			float z = myCam.nearClipPlane;
			float x = Mathf.Tan(myCam.fieldOfView / cushionSize) * z;
			float y = x / myCam.aspect;

			// Top Left
			intoArray[0] = (atRotation * new Vector3(-x, y, z)) + cameraPosition; // Added and rotated the point relative to camera
			// Top Right
			intoArray[1] = (atRotation * new Vector3(x, y, z)) + cameraPosition;
			// Bottom Left
			intoArray[2] = (atRotation * new Vector3(-x, -y, z)) + cameraPosition;
			// Bottom Right
			intoArray[3] = (atRotation * new Vector3(x, -y, z)) + cameraPosition;
			// Camera's Position
			intoArray[4] = cameraPosition - myCam.transform.forward;
		}


		bool CollisionDetectedAtClipPoints(Vector3[] clipPoints, Vector3 fromPosition)
		{
			for(int i = 0; i<clipPoints.Length; i++)
			{
				Ray ray = new Ray(fromPosition, clipPoints[i] - fromPosition);
				float distance = Vector3.Distance(clipPoints[i], fromPosition);

				if (Physics.Raycast(ray, distance, collisionLayer))
				{
					return true;
				}
			}
			return false;
		}

		public float GetAdjustedDistanceWithRayFrom(Vector3 from)
		{
			float distance = -1;

			for(int i = 0; i < desiredCameraClipPoints.Length; i++)
			{
				Ray ray = new Ray(from, desiredCameraClipPoints[i] - from);
				RaycastHit hit;

				if(Physics.Raycast(ray, out hit))
				{
					if(distance == -1)
					{
						distance = hit.distance;
					}
					else
					{
						if(hit.distance < distance)
						{
							distance = hit.distance;
						}
					}
				}
			}

			if(distance == -1)
			{
				return 0;
			}
			else
			{
				return distance;
			}
		}

		public void CheckColliding(Vector3 targetPosition)
		{
			if(CollisionDetectedAtClipPoints(desiredCameraClipPoints, targetPosition))
			{
				colliding = true;
			}
			else
			{
				colliding = false;
			}
		}
	}
}
