using UnityEngine;
using UnityEngine.UI;

namespace CC
{
    public class Option_Slider : MonoBehaviour, ICustomizerUI
    {
        public enum Type
        { Blendshape, Scalar, ModifyType };

        public Type CustomizationType;

        public CC_Property Property;

        public Vector2 Range = new Vector2(0, 1);
        public float DefaultValue = 0f;
        public string DisplayOption = "Option";
        public bool UsesDragCustomization;
        public float dragMultiplier = 1f;

        private Slider slider;

        private CharacterCustomization customizer;
        private CC_UI_Util parentUI;

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

            slider = GetComponentInChildren<Slider>();
            slider.minValue = Range.x;
            slider.maxValue = Range.y;
            slider.SetValueWithoutNotify(DefaultValue);
            GetComponentInChildren<TMPro.TMP_Text>(true).text = DisplayOption;
            gameObject.name = "Slider_" + DisplayOption;
        }

#endif

        public void InitializeUIElement(CharacterCustomization customizerScript, CC_UI_Util ParentUI)
        {
            slider = GetComponentInChildren<Slider>();
            customizer = customizerScript;
            parentUI = ParentUI;

            if (UsesDragCustomization) CC_UI_Manager.instance.onDrag += onBodyCustomization;

            slider.onValueChanged.AddListener(setProperty);

            RefreshUIElement();
        }

        private void onBodyCustomization(string partX, string partY, float deltaX, float deltaY, bool first, bool last)
        {
            if (partX == Property.propertyName)
            {
                slider.value += deltaX * dragMultiplier;
            }
            if (partY == Property.propertyName)
            {
                slider.value += deltaY * dragMultiplier;
            }
        }

        public void RefreshUIElement()
        {
            //Get saved value
            switch (CustomizationType)
            {
                case Type.Blendshape:
                case Type.ModifyType:
                    {
                        if (customizer.findProperty(customizer.StoredCharacterData.Blendshapes, Property, out Property, out int savedIndex))
                        {
                            slider.SetValueWithoutNotify(Property.floatValue);
                        }
                        else
                        {
                            slider.SetValueWithoutNotify(DefaultValue);
                        }
                        break;
                    }
                case Type.Scalar:
                    {
                        if (customizer.findProperty(customizer.StoredCharacterData.FloatProperties, Property, out Property, out int savedIndex))
                        {
                            slider.SetValueWithoutNotify(Property.floatValue);
                        }
                        else
                        {
                            slider.SetValueWithoutNotify(DefaultValue);
                        }
                        break;
                    }
            }
        }

        public void setProperty(float value)
        {
            Property.floatValue = value;

            switch (CustomizationType)
            {
                case Type.Blendshape:
                case Type.ModifyType:
                    {
                        customizer.setBlendshapeByName(Property.propertyName, value);
                        break;
                    }
                case Type.Scalar:
                    {
                        customizer.setFloatProperty(Property, true);
                        break;
                    }
            }
        }
    }
}