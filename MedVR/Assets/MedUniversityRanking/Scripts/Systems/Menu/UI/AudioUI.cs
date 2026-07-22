using Nato.PubSub;
using Nato.StateMachine;
using UnityEngine;
using UnityEngine.UI;
using static Nato.GameEvents;

namespace Nato.UI
{
    public class AudioUI : BaseUI
    {
        [Header("Buttons")]
        [field: SerializeField] public Button BackButton;

        protected override void Awake()
        {
            base.Awake();
            BackButton.onClick.AddListener(OnBackButtonClicked);
        }

        private void OnDestroy()
        {
            BackButton.onClick.RemoveListener(OnBackButtonClicked);
        }

        private void OnBackButtonClicked()
        {
            //EventManager<UIStateChangedEvent>.Publish(new UIStateChangedEvent(UIStates.Instance?.SettingsState));
            SetSelectedButton(LastPanelSelectButton);
        }
    }
}