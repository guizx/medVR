using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CC
{
    public class CopyPose : MonoBehaviour
    {
        public List<Transform> TargetBones = new List<Transform>();

        public void ParseBones(Dictionary<string, Transform> mainMeshBoneMap)
        {
            var targetHierarchy = GetRootBone(GetComponentInChildren<SkinnedMeshRenderer>().rootBone).GetComponentsInChildren<Transform>();

            var targetBonesDict = targetHierarchy.ToDictionary(t => t.name, t => t);

            foreach (var (boneName, sourceBone) in mainMeshBoneMap)
            {
                if (!targetBonesDict.TryGetValue(boneName, out var targetBone)) continue;

                //Re-parent & reset local transform
                targetBone.SetParent(sourceBone);
                targetBone.localPosition = Vector3.zero;
                targetBone.localRotation = Quaternion.identity;
                targetBone.localScale = Vector3.one;

                if (!TargetBones.Contains(targetBone)) TargetBones.Add(targetBone);
            }

            //Rename bones to avoid duplicate object names
            foreach (var bone in targetHierarchy)
            {
                bone.name += "_" + GetInstanceID();
            }
        }

        private Transform GetRootBone(Transform bone)
        {
            if (bone.parent == transform) return bone;
            return GetRootBone(bone.parent);
        }

        private void OnDestroy()
        {
            if (TargetBones == null) return;

            foreach (Transform t in TargetBones)
            {
                if (t != null)
                {
                    Destroy(t.gameObject);
                }
            }
        }
    }
}