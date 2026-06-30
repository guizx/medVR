using System.Collections.Generic;
using UnityEngine;

namespace CC
{
    public class HeadColliders : MonoBehaviour
    {
        public List<ColliderSetup> colliders = new List<ColliderSetup>();
        public List<GameObject> colliderObjects = new List<GameObject>();

        public void createColliders()
        {
            foreach (var colliderSetup in colliders)
            {
                if (colliderSetup.mirror)
                {
                    CreateColliderObject(colliderSetup.position, colliderSetup.label + "_l");
                    Vector3 mirroredPosition = new Vector3(colliderSetup.position.x, colliderSetup.position.y, -colliderSetup.position.z);
                    CreateColliderObject(mirroredPosition, colliderSetup.label + "_r");
                }
                else CreateColliderObject(colliderSetup.position, colliderSetup.label);
            }
        }

        private void CreateColliderObject(Vector3 localPosition, string label)
        {
            GameObject colliderObject = new(label);
            colliderObjects.Add(colliderObject);

            colliderObject.transform.SetParent(transform);

            colliderObject.transform.localPosition = localPosition;

            SphereCollider sphereCollider = colliderObject.AddComponent<SphereCollider>();
            sphereCollider.radius = 0.03f;

            var thisCollider = GetComponent<Collider>();
            if (thisCollider != null) Physics.IgnoreCollision(thisCollider, sphereCollider, true);
        }
    }

    [System.Serializable]
    public class ColliderSetup
    {
        public string label;
        public Vector3 position;
        public bool mirror;
    }
}