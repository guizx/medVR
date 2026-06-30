using System.Collections.Generic;

namespace CC
{
    [System.Serializable]
    public class CC_CharacterData
    {
        public string CharacterName;
        public string CharacterPrefab;
        public List<CC_Property> Blendshapes;
        public List<string> HairNames;
        public List<string> ApparelNames;
        public List<int> ApparelMaterials;
        public List<CC_Property> FloatProperties;
        public List<CC_Property> TextureProperties;
        public List<CC_Property> ColorProperties;
    }
}