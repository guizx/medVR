using System.Collections.Generic;
using UnityEngine;

namespace CC
{
    [CreateAssetMenu(fileName = "Hair", menuName = "ScriptableObjects/Hair")]
    public class scrObj_Hair : ScriptableObject
    {
        [System.Serializable]
        public class Hairstyle
        {
            public string Name;
            public GameObject Mesh;
            public Texture2D ShadowMap;
            public bool AddCopyPoseScript;
            public Sprite Icon;
        }

        public List<Hairstyle> Hairstyles = new List<Hairstyle>();
        public CC_Property SkinShadowMapProperty;
        public CC_Property HairTintProperty;
    }
}