using UnityEngine;
using UnityEngine.InputSystem;

namespace MedGames
{
    public class AdminUI : MonoBehaviour
    {
        public static AdminUI Instance;

        public GameObject AdminPanel;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Update()
        {
            if (Keyboard.current != null)
            {
                if (Keyboard.current.ctrlKey.isPressed && Keyboard.current.f2Key.wasPressedThisFrame)
                {
                    AdminPanel.SetActive(true);
                }
            }
        }
    }
}
