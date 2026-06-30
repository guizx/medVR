using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

namespace CC
{
    [CreateAssetMenu(fileName = "Standard Outfit Collection", menuName = "ScriptableObjects/Outfits Standard")]
    public class scrObj_Outfits_Standard : scrObj_Outfits
    {
        [System.Serializable]
        public struct Outfit_Definition
        {
            public string OutfitName;
            public List<Outfit_Options> OutfitOptions;
        }

        [System.Serializable]
        public struct Outfit_Options
        {
            public List<string> Options;
            public float DefaultChance;
            public bool MatchMaterials;
            public int IndexToMatch;
        }

        public List<Outfit_Definition> Outfits;

        public override bool GetRandomOutfit(CharacterCustomization script, out List<string> apparelOptions, out List<int> apparelMaterials)
        {
            if (Outfits.Count < 1)
            {
                Debug.LogError("Tried to set random outfit but no outfits have been defined");
                apparelOptions = null;
                apparelMaterials = null;
                return false;
            }

            var outfit = Outfits[Random.Range(0, Outfits.Count)]; //Get random outfit definition (each outfit definition can have multiple options per slot)

            apparelOptions = new List<string>();
            apparelMaterials = new List<int>();

            var slotsToHide = new HashSet<int>();

            //One Outfit_Options per apparel slot
            for (int i = 0; i < script.ApparelTables.Count; i++)
            {
                if (outfit.OutfitOptions.Count <= i)
                {
                    Debug.LogError("Outfit options not found");
                    return false;
                }

                //Get available options
                var options = outfit.OutfitOptions[i].Options;

                //Get random chance
                float rand = Random.Range(0f, 1f);

                //If no options available or if it rolls default, set default name
                if (options.Count <= 0 || rand < outfit.OutfitOptions[i].DefaultChance)
                {
                    apparelOptions.Add(script.DefaultApparel[i]);
                    apparelMaterials.Add(0);
                    continue;
                }

                //Otherwise get random option
                GetRandomApparel(script.ApparelTables[i], options, out string apparelOption, out int apparelMaterial, out HashSet<int> hiddenSlots);

                slotsToHide.UnionWith(hiddenSlots);
                apparelOptions.Add(apparelOption);
                apparelMaterials.Add(apparelMaterial);
            }

            //Remove hidden slots from apparel data
            foreach (var index in slotsToHide)
            {
                apparelOptions[index] = "";
            }

            //Match materials
            for (int i = 0; i < outfit.OutfitOptions.Count; i++)
            {
                if (outfit.OutfitOptions[i].MatchMaterials)
                {
                    apparelMaterials[i] = apparelMaterials[outfit.OutfitOptions[i].IndexToMatch];
                }
            }

            return true;
        }

#if UNITY_EDITOR

        [CustomPropertyDrawer(typeof(Outfit_Options))]
        public class OutfitOptionsDrawer : PropertyDrawer
        {
            private readonly string[] labels = {
            "Upper Body",
            "Lower Body",
            "Footwear",
            "Headwear",
            "Accessory"
        };

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                int index = GetElementIndex(property);
                string customLabel = (index >= 0 && index < labels.Length) ? labels[index] : $"Option {index}";

                EditorGUI.PropertyField(position, property, new GUIContent(customLabel), true);
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }

            private int GetElementIndex(SerializedProperty property)
            {
                string path = property.propertyPath;
                int start = path.LastIndexOf("[") + 1;
                int end = path.LastIndexOf("]");
                if (start > 0 && end > start)
                {
                    string indexStr = path.Substring(start, end - start);
                    if (int.TryParse(indexStr, out int index))
                        return index;
                }
                return -1;
            }
        }

#endif
    }
}