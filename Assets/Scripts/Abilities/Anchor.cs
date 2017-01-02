using UnityEngine;
using System.Collections;

public class Anchor : MonoBehaviour
{
	public GameObject anchorPrefab;
	public LayerMask collisionMask; // So we don't teleport inside a wall
	public float collisionRange;

	GameObject connectedAnchor;
	private Character character;
	private PlayerInput.Controller input { get { return character.input; } }
	void Start ()
	{
		// Retrieve required component(s)
		character = GetComponentInParent<Character>();
		if (character == null)
		{
			this.enabled = false;
			return;
		}

	}
	// Update is called once per frame
	void Update ()
	{
		if(input.kid.anchor.WasPressed)
		{
			AnchorManagement();
		}
	}

	public void AnchorManagement ()
	{
		// If there isn't an anchor placed, you place it
		if (connectedAnchor == null)
		{
			connectedAnchor = Instantiate(anchorPrefab, transform.position, anchorPrefab.transform.rotation) as GameObject;
			connectedAnchor.transform.parent = character.transform.parent;
		}
		// If there is one in the world, teleport to its position and destroy it
		else
		{
			//Checking if the anchor is inside a wall
			Collider[] walls = Physics.OverlapSphere(connectedAnchor.transform.position + new Vector3(0,collisionRange/2,0), collisionRange, collisionMask);
			if(walls.Length != 0)
			{
				Debug.Log("Colliding with something, teleportation aborted");
			}
			else
			{
				character.transform.position = connectedAnchor.transform.position;
				Destroy(connectedAnchor);
			}
			
		}
	}
}
