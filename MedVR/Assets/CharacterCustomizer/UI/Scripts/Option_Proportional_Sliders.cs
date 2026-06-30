using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CC
{
    public class Option_Proportional_Sliders : MonoBehaviour, ICustomizerUI
    {
        private CharacterCustomization customizer;
        private CC_UI_Util parentUI;

        public List<CC_Property> Properties = new List<CC_Property>();

        public bool RemoveText = true;
        public bool Vertical = true;

        public GameObject SliderObject;
        public Transform SliderContainer;

        private List<Slider> sliders = new List<Slider>();
        private List<Option_Slider> sliderScripts = new List<Option_Slider>();

        private float sliderSum;

        public void InitializeUIElement(CharacterCustomization customizerScript, CC_UI_Util ParentUI)
        {
            customizer = customizerScript;
            parentUI = ParentUI;

            foreach (var slider in sliderScripts)
            {
                Destroy(slider.gameObject);
            }

            sliderScripts.Clear();
            sliders.Clear();

            for (int i = 0; i < Properties.Count; i++)
            {
                //Create sliders, assign to reference and add delegate
                var sliderObj = Instantiate(SliderObject, SliderContainer);
                Option_Slider sliderScript = sliderObj.AddComponent<Option_Slider>();

                if (RemoveText) sliderObj.GetComponentInChildren<TMP_Text>().gameObject.SetActive(false);

                sliderScripts.Add(sliderScript);
                sliderScript.Property = Properties[i];
                sliderScript.CustomizationType = Option_Slider.Type.Blendshape;
                sliderScript.InitializeUIElement(customizerScript, ParentUI);

                Slider slider = sliderScript.GetComponentInChildren<Slider>();
                if (Vertical) slider.SetDirection(Slider.Direction.BottomToTop, true);
                sliders.Add(slider);
                slider.onValueChanged.AddListener(delegate { checkExcess(slider); });
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
        }

        public void RefreshUIElement()
        {
            foreach (var slider in sliderScripts)
            {
                slider.RefreshUIElement();
            }
        }

        public void checkExcess(Slider mainSlider)
        {
            sliderSum = 0;

            foreach (Slider slider in sliders)
            {
                sliderSum = slider.value + sliderSum;
            }

            if (sliderSum > 1)
            {
                for (int i = 0; i < sliders.Count; i++)
                {
                    if (mainSlider != sliders[i])
                    {
                        distributeExcess(sliderSum - mainSlider.value, sliderSum - 1, i);
                    }
                }
            }
        }

        public void distributeExcess(float sum, float excess, int index)
        {
            sliders[index].SetValueWithoutNotify(sliders[index].value - (sliders[index].value / sum * excess));
            sliderScripts[index].setProperty(sliders[index].value);
        }
    }
}