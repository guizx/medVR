using System.Collections;
using UnityEngine;

namespace CC
{
    [DefaultExecutionOrder(100)]
    public class PhysicsManager : MonoBehaviour
    {
        public Animator animator;
        private Rigidbody[] rigidBodies;
        private Collider[] colliders;
        private ModifyBone[] modifyBones;

        public bool useGravity = true;

        public bool ragdolling;
        public bool customizing; //If customizing, colliders should be activated for hover detection

        private void Awake()
        {
            if (animator == null) animator = gameObject.GetComponent<Animator>();
            rigidBodies = gameObject.GetComponentsInChildren<Rigidbody>();
            colliders = gameObject.GetComponentsInChildren<Collider>();
            modifyBones = gameObject.GetComponentsInChildren<ModifyBone>();

            //Set rigid bodies to kinematic at start
            foreach (var item in rigidBodies)
            {
                if (item.gameObject == gameObject) continue; //Skip main rigid body
                item.useGravity = useGravity;
                item.isKinematic = true;
            }

            //Disable colliders if not in customization mode
            foreach (var item in colliders)
            {
                item.enabled = customizing;
            }
        }

        private void Start()
        {
            if (ragdolling) StartCoroutine(ragdoll(true));
        }

        public void customizationSetup()
        {
            //Create head rig
            var headRig = GetComponentInChildren<HeadColliders>();
            if (headRig != null)
            {
                headRig.createColliders();
            }

            //Enable colliders for hover detection
            foreach (var item in colliders)
            {
                item.enabled = true;
            }

            customizing = true;
        }

        public IEnumerator ragdoll(bool shouldRagdoll)
        {
            ragdolling = shouldRagdoll;

            //Enable colliders if ragdolling
            foreach (var item in colliders)
            {
                item.enabled = ragdolling;
            }

            //Notify modifyBone scripts
            foreach (var item in modifyBones)
            {
                item.onSimulate(ragdolling);
            }

            if (ragdolling) yield return new WaitForFixedUpdate();

            //Enable physics
            foreach (var item in rigidBodies)
            {
                item.isKinematic = !ragdolling;
                item.angularVelocity = Vector3.zero;
            }

            //Disable animator when ragdolling
            if (animator != null) animator.enabled = !ragdolling;

            yield break;
        }
    }
}