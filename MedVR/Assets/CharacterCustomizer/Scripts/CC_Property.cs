using UnityEngine;

namespace CC
{
    [System.Serializable]
    public class CC_Property
    {
        public string propertyName = "";
        public string stringValue = "";
        public float floatValue = 0;
        public Color colorValue = Color.black;
        public int materialIndex = -1;
        public string meshTag = "";
    }
}