using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CC
{
    public class BlendshapeManager : MonoBehaviour
    {
        public Dictionary<string, int> NameToIndex = new Dictionary<string, int>();

        private SkinnedMeshRenderer mesh;

        public void parseBlendshapes()
        {
            mesh = gameObject.GetComponent<SkinnedMeshRenderer>();

            if (mesh.sharedMesh != null)
            {
                for (int i = 0; i < mesh.sharedMesh.blendShapeCount; i++)
                {
                    string[] split = mesh.sharedMesh.GetBlendShapeName(i).Split(".");
                    NameToIndex.Add(split[split.Length - 1], i);
                }
            }
        }

        public void setBlendshape(string name, float value)
        {
            if (NameToIndex.ContainsKey(name)) mesh.SetBlendShapeWeight(NameToIndex[name], value * 100);
        }
    }
}