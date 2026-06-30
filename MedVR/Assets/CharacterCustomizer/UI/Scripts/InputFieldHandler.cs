using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CC
{
    public class InputFieldHandler : MonoBehaviour, ICustomizerUI
    {
        private TMPro.TMP_InputField inputField;
        private CC_UI_Util script;

        private void Awake()
        {
            inputField = GetComponent<TMPro.TMP_InputField>();
        }

        public void InitializeUIElement(CharacterCustomization customizerScript, CC_UI_Util parentUI)
        {
            script = parentUI;
        }

        public void RefreshUIElement()
        {
        }

        public void InvokeMethod(string methodName)
        {
            // Get the method from CC_UI_Util that matches the name
            var method = typeof(CC_UI_Util).GetMethod(methodName);
            if (method != null && method.GetParameters().Length == 1 &&
                method.GetParameters()[0].ParameterType == typeof(string))
            {
                method.Invoke(script, new object[] { inputField.text });
            }
        }
    }
}