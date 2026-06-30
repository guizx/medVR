using UnityEngine;

namespace CC
{
    public class CC_UI_Util : MonoBehaviour
    {
        private CharacterCustomization customizer;

        //Initialize all child UI elements
        public void Initialize(CharacterCustomization customizerScript)
        {
            customizer = customizerScript;

            var interfaces = gameObject.GetComponentsInChildren<ICustomizerUI>(true);

            foreach (var element in interfaces)
            {
                element.InitializeUIElement(customizerScript, this);
            }
        }

        //Refresh UI elements, for example after loading a different preset
        public void refreshUI()
        {
            var interfaces = gameObject.GetComponentsInChildren<ICustomizerUI>(true);

            foreach (var element in interfaces)
            {
                element.RefreshUIElement();
            }
        }

        public void characterNext()
        {
            CC_UI_Manager.instance.characterNext();
        }

        public void characterPrev()
        {
            CC_UI_Manager.instance.characterPrev();
        }

        public void saveToPreset(string name)
        {
            customizer.SaveToPreset(name);
            Debug.Log($"Character {name} saved in Presets object");
        }

        public void saveToJSON(string name)
        {
            customizer.SaveToJSON(name);
            Debug.Log($"Character {name} saved in Assets/CharacterCustomizer.json");
        }

        public void loadCharacter()
        {
            customizer.TryLoadCharacter();
            refreshUI();
        }

        public void setCharacterName(string newName)
        {
            customizer.setCharacterName(newName);
        }

        public void setCharacterPreset(string preset)
        {
        }

        public void randomizeCharacter()
        {
            customizer.randomizeAll();
        }

        public void randomizeOutfit()
        {
            customizer.setRandomOutfit();
        }

        public void randomizeCharacterAndOutfit()
        {
            customizer.randomizeCharacterAndOutfit();
        }

        public void createPrefab(string name)
        {
#if UNITY_EDITOR
            if (customizer == null)
            {
                Debug.LogWarning("No GameObject selected.");
                return;
            }

            var prefabScript = GetComponent<CreatePrefab>();
            if (prefabScript != null) { prefabScript.createPrefab(name, customizer.gameObject, customizer.StoredCharacterData.CharacterPrefab); }
#endif
        }
    }
}