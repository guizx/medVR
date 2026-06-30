using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CC
{
    public class Option_Color_Picker : MonoBehaviour, ICustomizerUI
    {
        private CharacterCustomization customizer;
        private CC_UI_Util parentUI;

        public CC_Property Property;
        public bool useOpacity;
        public string DisplayOption = "Option";
        public GameObject hsvSliders;
        private Image[] imgs;

        public Image pickerIcon;

        private float h; private float s; private float v; private float a = 1f;

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

            GetComponentInChildren<TMPro.TMP_Text>().text = DisplayOption;
            gameObject.name = "ColorPicker_" + DisplayOption;
        }

#endif

        public void InitializeUIElement(CharacterCustomization customizerScript, CC_UI_Util ParentUI)
        {
            customizer = customizerScript;
            parentUI = ParentUI;

            RefreshUIElement();
        }

        public void RefreshUIElement()
        {
            //Get saved value
            if (customizer.findProperty(customizer.StoredCharacterData.ColorProperties, Property, out Property, out int savedIndex))
            {
                pickerIcon.color = new Color(Property.colorValue.r, Property.colorValue.g, Property.colorValue.b, 1);
            }
        }

        public void toggleSliders()
        {
            Color.RGBToHSV(Property.colorValue, out h, out s, out v);
            a = Property.colorValue.a;
            var sliderObj = Instantiate(hsvSliders, parentUI.transform);

            //Remove on click
            var eventTrigger = sliderObj.GetComponentInChildren<EventTrigger>();
            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback = new EventTrigger.TriggerEvent();
            entry.callback.AddListener(a => Destroy(sliderObj));
            eventTrigger.triggers.Add(entry);

            //Put slider box on this transform
            var sliderContainer = sliderObj.transform.GetChild(1);
            sliderContainer.position = transform.position;

            var sliders = sliderObj.GetComponentsInChildren<Slider>();
            imgs = sliderObj.GetComponentsInChildren<Image>();

            sliders[0].SetValueWithoutNotify(h);
            sliders[1].SetValueWithoutNotify(s);
            sliders[2].SetValueWithoutNotify(v);
            sliders[3].SetValueWithoutNotify(a);

            sliders[0].onValueChanged.AddListener(f => { h = f; setColor(); });
            sliders[1].onValueChanged.AddListener(f => { s = f; setColor(); });
            sliders[2].onValueChanged.AddListener(f => { v = f; setColor(); });
            sliders[3].onValueChanged.AddListener(f => { a = f; setColor(); });

            foreach (var img in imgs)
            {
                if (!img.raycastTarget) img.color = Color.HSVToRGB(h, 1, 1);
            }

            if (!useOpacity)
            {
                Destroy(sliders[3].transform.parent.gameObject);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(sliderContainer.GetComponent<RectTransform>());
        }

        public void setColor()
        {
            Property.colorValue = Color.HSVToRGB(h, s, v);
            Property.colorValue.a = a;

            customizer.setColorProperty(Property, true);

            pickerIcon.color = new Color(Property.colorValue.r, Property.colorValue.g, Property.colorValue.b, 1);

            foreach (var img in imgs)
            {
                if (!img.raycastTarget) img.color = Color.HSVToRGB(h, 1, 1);
            }
        }
    }
}