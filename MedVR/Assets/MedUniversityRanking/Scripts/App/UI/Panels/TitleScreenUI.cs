using Nato.StateMachine;
using Nato.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace MedGames
{
    public class TitleScreenUI : BaseUI
    {
        [field: Header("Buttons")]
        [field: SerializeField] public Button StartButton { get; private set; }
        [field: SerializeField] public Button AddUniversityButton { get; private set; }

        public override void Enable()
        {
            base.Enable();
            StartButton.onClick.AddListener(OnClickStartButton);
            AddUniversityButton.onClick.AddListener(OnClickAddUniversityButton);
        }

        public override void Disable()
        {
            base.Disable();
            StartButton.onClick.RemoveListener(OnClickStartButton);
            AddUniversityButton.onClick.RemoveListener(OnClickAddUniversityButton);
        }

        private void OnClickAddUniversityButton()
        {
            UIUniversityCreateState universityCreateState = UIStates.Instance.UniversityCreateState;
            UIStateManager.Instance.StateMachine.TransitionTo(universityCreateState);
        }

        private void OnClickStartButton()
        {
            UIUFSelectionState UFSelectionState = UIStates.Instance.UFSelectionState;
            UIStateManager.Instance.StateMachine.TransitionTo(UFSelectionState);
        }
    }
}
