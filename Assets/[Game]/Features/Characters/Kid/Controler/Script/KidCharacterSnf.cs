using UnityEngine;
using System.Collections;

namespace Gameplay
{
    public class KidCharacterSnf : MonoBehaviour
    {

        public Animator animator;

        float moveSpeed = 0;

        public void SetRotation(Quaternion rotation)
        {
            if(moveSpeed >= 0.1f)
            {
                transform.rotation = rotation;
            }
        }

        public void SetMoveSpeed(float ms)
        {
            moveSpeed = ms;
            animator.SetFloat("moveSpeed", ms);
        }

    }
}