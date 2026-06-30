using System.Collections.Generic;
using UnityEngine;

namespace CC
{
    [CreateAssetMenu(fileName = "Presets", menuName = "ScriptableObjects/Presets")]
    public class scrObj_Presets : ScriptableObject
    {
        public List<CC_CharacterData> Presets = new List<CC_CharacterData>();
    }
}