using UnityEngine;
using System.Collections;

public class LM_Trigger : MonoBehaviour
{

	public GameObject toOpen;
	public bool deActivateOnLeave;

        private void OnTriggerEnter(Collider other)
    {
        toOpen.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
		if(deActivateOnLeave)
		{
			toOpen.SetActive(true);
		}
    }
}
