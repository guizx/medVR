using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CC
{
    public class Tab_Manager : MonoBehaviour
    {
        [Header("Button Active Colors")]
        public ColorBlock TabColorActive;

        [Header("Button Inactive Colors")]
        public ColorBlock TabColorInactive;

        public GameObject TabParent;
        private List<GameObject> tabs = new List<GameObject>();
        private List<GameObject> tabMenus = new List<GameObject>();

        private SmoothScroll scrollRect;
        public SmoothScroll targetScroll;

        private void Start()
        {
            assignTabs();
        }

        public void assignTabs()
        {
            tabs.Clear();
            tabMenus.Clear();

            for (int i = 0; i < transform.childCount; i++)
            {
                var tab = transform.GetChild(i).gameObject;
                var index = i;
                tabs.Add(tab);

                tab.GetComponentInChildren<Button>().onClick.AddListener(() => switchTab(index));
            }

            if (TabParent != null) foreach (Transform child in TabParent.transform)
                {
                    tabMenus.Add(child.gameObject);
                }

            scrollRect = GetComponentInParent<SmoothScroll>();
            switchTab(0);
            if (scrollRect != null) scrollRect.resetScroll(true);
        }

        public void switchTab(int tab)
        {
            for (int i = 0; i < tabs.Count; i++)
            {
                //Set tab color
                tabs[i].GetComponentInChildren<Button>().colors = tab == i ? TabColorActive : TabColorInactive;

                //Set tab active state
                if (tabMenus.Count > i) tabMenus[i].SetActive(tab == i);
            }

            if (scrollRect != null) scrollRect.ScrollToContent(tabs[tab].GetComponent<RectTransform>());
            if (TabParent != null) LayoutRebuilder.ForceRebuildLayoutImmediate(TabParent.GetComponent<RectTransform>());
            if (targetScroll != null) targetScroll.resetScroll();
        }
    }
}