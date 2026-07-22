using Nato.StateMachine;
using Nato.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MedGames
{
    public class MainScreenUI : BaseUI
    {
        [field: Header("UI Elements")]
        [field: SerializeField] public Image UniversityIconImage { get; private set; }
        [field: SerializeField] public TextMeshProUGUI UniversityNameText { get; private set; }

        [field: Header("Buttons")]
        [field: SerializeField] public Button RankingButton { get; private set; }
        [field: SerializeField] public Button BackButton { get; private set; }

        public override void Enable()
        {
            base.Enable();
            //UniversityIconImage.sprite = QuizManager.Instance.CurrentUniversity.UniversityData.Icon;
            UniversityDTO universitySelected = JsonDatabaseManager.Instance.UniversityDatabase.UniversitySelected;
            UniversityNameText.text = $"{universitySelected.sigla} - {universitySelected.ies}";

            RankingButton.onClick.AddListener(OnClickRankingButton);
            BackButton.onClick.AddListener(OnClickBackButton);
        }

        public override void Disable()
        {
            base.Disable();
            RankingButton.onClick.RemoveListener(OnClickRankingButton);
            BackButton.onClick.RemoveListener(OnClickBackButton);
        }

        private void OnClickRankingButton()
        {
            UIRankingState rankingState = UIStates.Instance.RankingState;
            UIStateManager.Instance.StateMachine.TransitionTo(rankingState);
            rankingState.Manager.UIPanels.RankingUI.SetButtonText("Voltar");
            rankingState.Manager.UIPanels.RankingUI.BackButton.onClick.AddListener(() =>
            {
                UIMainScreenState mainScreenState = UIStates.Instance.MainScreenState;
                UIStateManager.Instance.StateMachine.TransitionTo(mainScreenState);
            });
        }

        private void OnClickBackButton()
        {
            UIUniversitySelectionState universitySelectionState = UIStates.Instance.UniversitySelectionState;
            UIStateManager.Instance.StateMachine.TransitionTo(universitySelectionState);
        }
    }
}
