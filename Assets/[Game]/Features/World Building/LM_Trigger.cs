using UnityEngine;
using System.Collections;

public class LM_Trigger : MonoBehaviour
{
	[FMODUnity.EventRef]
	public string openDoor = "event:/Global/OnDoorOpen";
	[FMODUnity.EventRef]
	public string closeDoor = "event:/Global/OnDoorClose";
	[FMODUnity.EventRef]
	public string trigger = "event:/Global/OnTriggerActivate";

	public GameObject toOpen;
	public bool deActivateOnLeave;

        private void OnTriggerEnter(Collider other)
    {
		FMODUnity.RuntimeManager.PlayOneShot(trigger, transform.position);
		FMODUnity.RuntimeManager.PlayOneShot(openDoor, toOpen.transform.position);
		toOpen.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
		if(deActivateOnLeave)
		{
			FMODUnity.RuntimeManager.PlayOneShot(trigger, transform.position);
			FMODUnity.RuntimeManager.PlayOneShot(closeDoor, toOpen.transform.position);
			toOpen.SetActive(true);
		}
    }
}
