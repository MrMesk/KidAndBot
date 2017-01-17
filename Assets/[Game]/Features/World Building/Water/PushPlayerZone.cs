using UnityEngine;
using System.Collections;

namespace Gameplay.Water
{
    public class PushPlayerZone : MonoBehaviour
    {
        public Vector3 velocity;

        public void OnTriggerStay(Collider other)
        {
            KidCharacter kid = other.GetComponent<KidCharacter>();
            if(kid != null)
            {
                kid.physic.globalVelocity = velocity;
            }
        }
    }
}