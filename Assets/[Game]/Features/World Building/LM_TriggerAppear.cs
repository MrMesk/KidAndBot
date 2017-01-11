using UnityEngine;
using System.Collections;

public class LM_TriggerAppear : MonoBehaviour {

    public GameObject toAppear;

    private void OnTriggerEnter(Collider other)
    {
        toAppear.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        toAppear.SetActive(false);
    }
}
