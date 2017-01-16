using UnityEngine;
using System.Collections;
using PlayerInput;
using System.Collections.Generic;

namespace Gameplay
{

    public partial class KidCharacter : Character
    {

        /*********
         * LOGIC *
         *********/

        public class Attach
        {
            public AttachNode attachNode;

            public bool IsAttached()
            {
                return attachNode != null;
            }

            public bool IsAttachedTo(AttachNode attachNode)
            {
                return this.attachNode == attachNode;
            }
        }

        [System.NonSerialized]
        public Attach attach = new Attach();

        /// <summary>
        /// Initialise jump related logic.
        /// </summary>
        private void Attach_Init()
        {
            // Bind to events
            eOnDestroy += Attach_OnDestroy;
            AttachNode.eOnKidEnter += Attach_OnKidEnterAttachNode;
            AttachNode.eOnKidExit += Attach_OnKidExitAttachNode;
        }

        private void Attach_OnDestroy()
        {
            // Unbind from events
            eOnDestroy -= Attach_OnDestroy;
            AttachNode.eOnKidEnter -= Attach_OnKidEnterAttachNode;
            AttachNode.eOnKidExit -= Attach_OnKidExitAttachNode;
        }

        private void Attach_LogicTick(float dt)
        {
            if (attach.IsAttached() && !Jump_IsJumping())
            {
                Attach_MoveTowardAttachedNode(dt);
            }
        }

        private void Attach_MoveTowardAttachedNode(float dt)
        {
            Vector3 postion = transform.position;
            Vector3 target = attach.attachNode.transform.position;
            float dist = Vector3.Distance(postion, target);
            postion = Vector3.MoveTowards(
                postion,
                target,
                dt *
                (
                    16f * dist +
                    0f
                )
                );
            Vector3 translation = postion - transform.position;
            physic.translate += translation;
        }

        private void Attach_AttachTo(AttachNode attachNode)
        {
            if (attach.IsAttached())
            {
                Attach_DetachFrom(attach.attachNode);
            }
            if (attachNode.eOnKidAttach != null)
            {
                attachNode.eOnKidAttach();
            }
            attach.attachNode = attachNode;

            // End activ jump
            if (Jump_IsJumping())
            {
                Jump_EndActiveJump();
            }
        }

        private void Attach_DetachFrom(AttachNode attachNode)
        {
            if (attachNode.eOnKidDetach != null)
            {
                attachNode.eOnKidDetach();
            }
            attach.attachNode = null;
        }

        /**********
         * PHYSIC *
         **********/

        private void Attach_OnKidEnterAttachNode(AttachNode attachNode)
        {
            Attach_AttachTo(attachNode);
        }

        private void Attach_OnKidExitAttachNode(AttachNode attachNode)
        {
            if (attach.IsAttachedTo(attachNode))
            {
                Attach_DetachFrom(attachNode);
            }
        }


        /**************
         * CONTROLLER *
         **************/

    }
}