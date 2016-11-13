using UnityEngine;
using System.Collections;

public class Climbable : MonoBehaviour 
{

    public BoxCollider box;

    private void Awake() 
	{
        box = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other) 
	{
        Character character = other.GetComponent<Character>();
        if(character != null) 
		{
            //character.Climb(this);
        }
    }

}
