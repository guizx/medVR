using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace CC
{
    public class Apparel_Menu : MonoBehaviour, ICustomizerUI
    {
        public GameObject ButtonPrefab;
        public GameObject Container;

        private CharacterCustomization customizer;

        public bool useIcons = true;
        public Sprite defaultIcon;

        public GameObject CategoryPrefab;
        public GameObject CategoryContainer;

        private List<scrObj_Apparel.MenuCategory> menuCategories;

        public SmoothScroll CategoryScroll;

        public void InitializeUIElement(CharacterCustomization customizerScript, CC_UI_Util parentUI)
        {
            customizer = customizerScript;
            RefreshUIElement();
        }

        public void createApparelButtons(scrObj_Apparel.MenuCategory menuCategory)
        {
            foreach (Transform child in Container.transform)
            {
                Destroy(child.gameObject);
            }
            for (int i = 0; i < customizer.ApparelTables.Count; i++)
            {
                var items = customizer.ApparelTables[i].Items.Where(item => item.MenuCategory == menuCategory);

                foreach (var item in items)
                {
                    for (int j = 0; j < item.Materials.Count; j++)
                    {
                        createButton(item.Name, i, j, item.Materials[j].Icon);
                    }

                    if (item.Materials.Count == 0)
                    {
                        createButton(item.Name, i, 0, null);
                    }
                }
            }
        }

        private void createCategoryButtons()
        {
            //Safely destroy existing category buttons
            var children = new List<GameObject>();
            foreach (Transform child in CategoryContainer.transform) children.Add(child.gameObject);
            foreach (var go in children)
            {
                go.transform.SetParent(null);
                Destroy(go);
            }

            //Try get tab manager
            var tabManager = CategoryContainer.GetComponentInParent<Tab_Manager>();

            //Get menu categories in order
            menuCategories = customizer.ApparelTables.SelectMany(table => table.Items).Select(item => item.MenuCategory).Distinct().ToList();

            //Create category button per menu category
            for (int i = 0; i < menuCategories.Count; i++)
            {
                scrObj_Apparel.MenuCategory cat = menuCategories[i];
                GameObject categoryButton = Instantiate(CategoryPrefab, CategoryContainer.transform).gameObject;
                categoryButton.GetComponentInChildren<TextMeshProUGUI>().text = menuCategories[i].ToString();
                var button = categoryButton.GetComponentInChildren<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => { createApparelButtons(cat); });
            }

            if (tabManager != null) tabManager.assignTabs();
        }

        private void createButton(string text, int slot, int material, Sprite sprite)
        {
            string name = text;
            int matIndex = material;
            int apparelSlot = slot;

            GameObject Button = Instantiate(ButtonPrefab, Container.transform).gameObject;
            Button.GetComponentInChildren<Button>().onClick.AddListener(() => { customizer.setApparelByName(name, apparelSlot, matIndex); });

            if (useIcons) Button.GetComponentInChildren<Image>().sprite = sprite == null ? defaultIcon : sprite;
            else Button.GetComponentInChildren<TextMeshProUGUI>().text = text;
        }

        public void RefreshUIElement()
        {
            createCategoryButtons();
            createApparelButtons(menuCategories[0]);
            if (CategoryScroll != null) CategoryScroll.resetScroll(true);
        }
    }
}