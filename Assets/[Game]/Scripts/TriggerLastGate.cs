using UnityEngine;
using System.Collections;

public class TriggerLastGate : MonoBehaviour {

    public Animator lastGateAnimator;

    public void OnTriggerEnter(Collider other)
    {
        Gameplay.KidCharacter kid = other.GetComponent<Gameplay.KidCharacter>();
        if(kid != null)
        {
            lastGateAnimator.SetBool("open", true);
        }
    }

}
