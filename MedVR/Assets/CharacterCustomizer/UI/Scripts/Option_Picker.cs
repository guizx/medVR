using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CC
{
    public class Option_Picker : MonoBehaviour, ICustomizerUI
    {
        private CharacterCustomization customizer;

        public enum Type
        { Blendshape, Texture, Hair, Color, Stencil };

        public Type CustomizationType;

        public CC_Property Property;
        public List<CC_Property> Options = new List<CC_Property>();
        public int Slot = 0;

        public TextMeshProUGUI PropertyText;
        public TextMeshProUGUI OptionText;

        public string DisplayOption;

        private int navIndex = 0;
        private int optionsCount = 0;

        public bool valueFromIndex = true;
        public bool valueFromString = false;
        public bool blendshapeUseOptionValue;
        public List<CC_Stencil> stencilOptions = new List<CC_Stencil>();

        public void InitializeUIElement(CharacterCustomization customizerScript, CC_UI_Util ParentUI)
        {
            customizer = customizerScript;
            RefreshUIElement();
            foreach (var item in stencilOptions)
            {
                item.basePropertyName = Property.propertyName;
                item.materialIndex = Property.materialIndex;
                item.meshTag = Property.meshTag;
            }

            if (!blendshapeUseOptionValue && CustomizationType == Type.Blendshape) return;

            foreach (var item in Options)
            {
                item.propertyName = Property.propertyName;
                item.materialIndex = Property.materialIndex;
                item.meshTag = Property.meshTag;
            }
        }

        public void RefreshUIElement()
        {
            switch (CustomizationType)
            {
                case Type.Blendshape:
                    {
                        optionsCount = Options.Count;
                        updateOptionText();
                        for (int i = 0; i < Options.Count; i++)
                        {
                            CC_Property prop;
                            if (customizer.findProperty(customizer.StoredCharacterData.Blendshapes, Options[i], out prop, out int savedIndex))
                            {
                                if (prop.floatValue != 0)
                                {
                                    navIndex = i;
                                    updateOptionText();
                                    break;
                                }
                            }
                        }
                        break;
                    }
                case Type.Texture:
                    {
                        optionsCount = Options.Count;
                        if (customizer.findProperty(customizer.StoredCharacterData.TextureProperties, Property, out Property, out int savedIndex))
                        {
                            navIndex = Options.FindIndex(t => t.stringValue == Property.stringValue);
                        }
                        else navIndex = 0;
                        updateOptionText();
                        break;
                    }
                case Type.Stencil:
                    {
                        optionsCount = stencilOptions.Count;
                        if (customizer.findProperty(customizer.StoredCharacterData.TextureProperties, Property, out Property, out int savedIndex))
                        {
                            navIndex = stencilOptions.FindIndex(t => t.texture.name == Property.stringValue);
                        }
                        else navIndex = 0;
                        updateOptionText();
                        break;
                    }
                case Type.Color:
                    {
                        optionsCount = Options.Count;
                        if (customizer.findProperty(customizer.StoredCharacterData.ColorProperties, Property, out Property, out int savedIndex))
                        {
                            navIndex = Options.FindIndex(t => colorMatch(t.colorValue, Property.colorValue));
                            if (navIndex == -1) navIndex = 0;
                        }
                        else navIndex = 0;
                        updateOptionText();
                        break;
                    }
                case Type.Hair:
                    {
                        if (customizer.HairTables.Count <= Slot)
                        {
                            Destroy(gameObject);
                            return;
                        }

                        optionsCount = customizer.HairTables[Slot].Hairstyles.Count;
                        navIndex = customizer.HairTables[Slot].Hairstyles.FindIndex(t => t.Name == customizer.StoredCharacterData.HairNames[Slot]);
                        if (navIndex == -1) navIndex = 0;
                        updateOptionText();
                        break;
                    }
            }
        }

        private bool colorMatch(Color a, Color b, float tolerance = 0.1f)
        {
            return Mathf.Abs(a.r - b.r) < tolerance &&
                   Mathf.Abs(a.g - b.g) < tolerance &&
                   Mathf.Abs(a.b - b.b) < tolerance;
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            UnityEditor.EditorApplication.delayCall += OnValidateCallback;
        }

        private void OnValidateCallback()
        {
            if (this == null || Application.isPlaying)
            {
                UnityEditor.EditorApplication.delayCall -= OnValidateCallback;
                return;
            }

            PropertyText.text = DisplayOption;
            if (DisplayOption != "") gameObject.name = "Picker_" + DisplayOption;
        }

#endif

        public void updateOptionText()
        {
            if (valueFromIndex) { OptionText.gameObject.SetActive(true); OptionText.SetText((navIndex + 1) + "/" + optionsCount); }
            if (valueFromString) { OptionText.gameObject.SetActive(true); OptionText.SetText(Options[navIndex].stringValue); }
        }

        public void setOption(int i)
        {
            navIndex = i;

            updateOptionText();

            switch (CustomizationType)
            {
                case Type.Blendshape:
                    {
                        if (blendshapeUseOptionValue)
                        {
                            customizer.setBlendshapeByName(Property.propertyName, Options[i].floatValue);
                            break;
                        }
                        foreach (var p in Options)
                        {
                            customizer.setBlendshapeByName(name, 0);
                        }
                        customizer.setBlendshapeByName(Options[i].propertyName, 1);
                        break;
                    }
                case Type.Hair:
                    {
                        customizer.setHair(i, Slot);
                        break;
                    }
                case Type.Texture:
                    {
                        var pNames = Options[i].propertyName.Split(",");
                        var pValues = Options[i].stringValue.Split(",");
                        var pTags = Options[i].meshTag.Split(",");
                        for (int j = 0; j < pNames.Length; j++)
                        {
                            string value = j < pValues.Length ? pValues[j] : string.Empty;
                            string tag = j < pTags.Length ? pTags[j] : string.Empty;
                            var prop2 = new CC_Property() { propertyName = pNames[j], stringValue = value, materialIndex = Property.materialIndex, meshTag = tag };
                            customizer.setTextureProperty(prop2, true);
                        }
                        break;
                    }
                case Type.Stencil:
                    var stencil = stencilOptions[i];
                    var textureProp = new CC_Property() { propertyName = stencil.basePropertyName, materialIndex = stencil.materialIndex, meshTag = stencil.meshTag };
                    customizer.setTextureProperty(textureProp, true, stencil.texture);
                    var offsetX = new CC_Property() { propertyName = stencil.basePropertyName + "_Offset_X", floatValue = stencil.offsetX, materialIndex = stencil.materialIndex, meshTag = stencil.meshTag };
                    customizer.setFloatProperty(offsetX, true);
                    var offsetY = new CC_Property() { propertyName = stencil.basePropertyName + "_Offset_Y", floatValue = stencil.offsetY, materialIndex = stencil.materialIndex, meshTag = stencil.meshTag };
                    customizer.setFloatProperty(offsetY, true);
                    var scaleX = new CC_Property() { propertyName = stencil.basePropertyName + "_Scale_X", floatValue = stencil.scaleX, materialIndex = stencil.materialIndex, meshTag = stencil.meshTag };
                    customizer.setFloatProperty(scaleX, true);
                    var scaleY = new CC_Property() { propertyName = stencil.basePropertyName + "_Scale_Y", floatValue = stencil.scaleY, materialIndex = stencil.materialIndex, meshTag = stencil.meshTag };
                    customizer.setFloatProperty(scaleY, true);
                    var rotProp = new CC_Property() { propertyName = stencil.basePropertyName + "_Rotation", floatValue = stencil.rotation, materialIndex = stencil.materialIndex, meshTag = stencil.meshTag };
                    customizer.setFloatProperty(rotProp, true);
                    var tintableProp = new CC_Property() { propertyName = stencil.basePropertyName + "_Tintable", floatValue = stencil.tintable ? 1 : 0, materialIndex = stencil.materialIndex, meshTag = stencil.meshTag };
                    customizer.setFloatProperty(tintableProp, true);

                    break;

                case Type.Color:
                    {
                        customizer.setColorProperty(Options[i], true);
                        break;
                    }
            }
        }

        public void navLeft()
        {
            setOption(navIndex == 0 ? optionsCount - 1 : navIndex - 1);
        }

        public void navRight()
        {
            setOption(navIndex == optionsCount - 1 ? 0 : navIndex + 1);
        }
    }
}