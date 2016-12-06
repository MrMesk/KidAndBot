using UnityEngine;
using System.Collections;

public class LM_Trigger : MonoBehaviour {

	public GameObject toOpen;

        private void OnTriggerEnter(Collider other)
    {
        toOpen.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        toOpen.SetActive(true);
    }
}
