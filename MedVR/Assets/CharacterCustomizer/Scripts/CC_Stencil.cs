using UnityEngine;

namespace CC
{
    [System.Serializable]
    public class CC_Stencil
    {
        public string basePropertyName = "";
        public Texture2D texture;
        public float offsetX;
        public float offsetY;
        public float scaleX;
        public float scaleY;
        public float rotation = 0;
        public bool tintable;
        public int materialIndex = -1;
        public string meshTag = "";
    }
}