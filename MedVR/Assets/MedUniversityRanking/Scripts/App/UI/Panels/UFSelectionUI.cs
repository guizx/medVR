using Nato.StateMachine;
using Nato.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MedGames
{
    public class UFSelectionUI : BaseUI
    {
        [field: Header("Lists")]
        [field: SerializeField] public List<UFButton> UFButtons = new List<UFButton>();

        [field: Header("Containers")]
        [SerializeField] private Transform verticalContainer;

        [field: Header("UI Elements")]
        [field: SerializeField] public TMP_InputField FilterInputField { get; private set; }
        [field: SerializeField] public ScrollRect ScrollRectUniversities { get; private set; }
        [field: SerializeField] public Button BackButton { get; private set; }

        protected override void Awake()
        {
            base.Awake();
        }

        public override void Enable()
        {
            base.Enable();
            BackButton.onClick.AddListener(OnClickBackButton);
        }

        public override void Disable()
        {
            base.Disable();
            BackButton.onClick.RemoveListener(OnClickBackButton);
        }

        private void OnClickBackButton()
        {
            UITitleScreenState titleScreenState = UIStates.Instance.TitleScreenState;
            UIStateManager.Instance.StateMachine.TransitionTo(titleScreenState);
        }
    }
}
