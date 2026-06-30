using System.Collections.Generic;
using UnityEngine;

namespace CC
{
    [System.Serializable]
    public class CC_Apparel_Material_Collection
    {
        public string Label;
        public List<CC_Apparel_Material_Definition> MaterialDefinitions = new List<CC_Apparel_Material_Definition>();
        public Sprite Icon;
    }
}