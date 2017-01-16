using UnityEngine;
using System.Collections;

namespace Gameplay
{
    public class AttachNode : MonoBehaviour
    {        
        /**********
         * CONFIG *
         **********/
        [Header("References")]
        public KidCharacter kid;
        [Header("Config")]
        public float range;

        /**********
         * EVENTS *
         **********/
        private bool _kidIsInRange = false;
        private bool _attached = false;

        /**********
         * EVENTS *
         **********/
        public delegate void AttachNodeEventHandler(AttachNode attachNode);
        // Enter / Exit
        public static AttachNodeEventHandler eOnKidEnter;
        public static AttachNodeEventHandler eOnKidExit;
        // Attach / Detach
        public System.Action eOnKidAttach;
        public System.Action eOnKidDetach;

        /*********
         * UNITY *
         *********/

        private void Awake()
        {
            Logic_Init();
        }

        private void OnDestroy()
        {
            Logic_End();
        }

        private void FixedUpdate()
        {
            Logic_Tick(Time.fixedDeltaTime);
        }

        /*********
         * LOGIC *
         *********/

        private void Logic_Init()
        {
            eOnKidAttach += OnKidAttach;
            eOnKidDetach += OnKidDetach;
        }

        private void Logic_End()
        {
            eOnKidAttach -= OnKidAttach;
            eOnKidDetach -= OnKidDetach;
        }

        private void Logic_Tick(float dt)
        {
            bool kidIsInRange = IsInRange(kid.transform);
            if (IsInRange(kid.transform))
            {
                if (_kidIsInRange == false)
                {
                    OnKidEnter();
                }
            }
            else
            {
                if (_kidIsInRange == true)
                {
                    OnKidExit();
                }
            }
            _kidIsInRange = kidIsInRange;
        }

        /******************
         * EVENT CALLBACK *
         ******************/
        private void OnKidEnter()
        {
            if(eOnKidEnter != null)
            {
                eOnKidEnter(this);
            }
        }

        private void OnKidExit()
        {
            if (eOnKidExit != null)
            {
                eOnKidExit(this);
            }
        }

        /******************
         * EVENT HANDLING *
         ******************/
        public void OnKidAttach()
        {
            _attached = true;
        }

        public void OnKidDetach()
        {
            _attached = false;
        }

        /// <summary>
        /// Return if the transform is inside the range of this node.
        /// </summary>
        public bool IsInRange(Transform transform)
        {
            return Vector3.Distance(transform.position, this.transform.position) <= range;
        }

        /*************
         * GET / SET *
         *************/
        /// <summary>
        /// Return if the kid is attached to this node.
        /// </summary>
        public bool IsAttached()
        {
            return _attached;
        }

    }
}