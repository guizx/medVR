using System.Collections.Generic;
using UnityEngine;

namespace CC
{
    [CreateAssetMenu(fileName = "Apparel", menuName = "ScriptableObjects/Apparel")]
    public class scrObj_Apparel : ScriptableObject
    {
        public enum MenuCategory
        { Outfit, Upper_Body, Lower_Body, Footwear, Headwear, Hands, Jewelry, Rings, Backpacks, Misc, Glasses, Tops, Pants, Shorts, Accessories, Jackets, Underwear, Dresses, Swimwear, Skirts, Uniform };

        [System.Serializable]
        public class Apparel
        {
            public string Name;
            public GameObject Mesh;
            public string DisplayName;
            public bool AddCopyPoseScript;
            public Texture2D Mask;
            public FootOffset FootOffset = new FootOffset() { HeightOffset = -1, FootRotation = 0, BallRotation = 0 };
            public List<CC_Apparel_Material_Collection> Materials;
            public float NeckShrink = -1;
            public float CompressHair = -1;
            public List<int> HidesTheseSlots;
            public MenuCategory MenuCategory;
        }

        public List<Apparel> Items = new List<Apparel>();
        public CC_Property SkinMaskProperty;
        public string Label;
    }
}