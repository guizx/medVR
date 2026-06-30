using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CC
{
    public class ClickAudio : MonoBehaviour
    {
        public int sound;

        private void Start()
        {
            var buttons = GetComponentsInChildren<Button>();
            foreach (var button in buttons)
            {
                button.onClick.AddListener(() => CC_UI_Manager.instance.playUIAudio(sound));
            }
        }
    }
}