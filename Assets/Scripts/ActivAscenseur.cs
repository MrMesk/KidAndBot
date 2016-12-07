using UnityEngine;
using System.Collections;

public class ActivAscenseur : MonoBehaviour {


	Ascenseur _scriptAscenseur;
	public Vector3 maxPos ;
    public Vector3 minPos;

	public GameObject ascenseur;

	// Use this for initialization
	void Start () 
	{
		_scriptAscenseur = ascenseur.GetComponent<Ascenseur>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter (Collider col)
	{
		
		_scriptAscenseur.StartCoroutine(_scriptAscenseur.Ascend( maxPos, minPos));


		this.enabled = false;
	}
}
