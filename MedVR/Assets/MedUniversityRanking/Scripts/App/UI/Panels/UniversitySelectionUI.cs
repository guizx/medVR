using Nato.StateMachine;
using Nato.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MedGames
{
    public class UniversitySelectionUI : BaseUI
    {
        [Header("Data")]
        [SerializeField] private List<UniversityDTO> Universities = new List<UniversityDTO>();
        public List<UniversityRanking> UniversityRankings;

        [Header("Prefabs")]
        [field: SerializeField] public UniversityButton UniversityButtonPrefab;

        [Header("Lists")]
        [field: SerializeField] public List<UniversityButton> UniversityButtons = new List<UniversityButton>();

        [Header("Containers")]
        [SerializeField] private Transform verticalContainer;

        [Header("UI Elements")]
        [field: SerializeField] public Button RankingButton { get; private set; }
        [field: SerializeField] public Button BackButton { get; private set; }
        [field: SerializeField] public TMP_InputField FilterInputField { get; private set; }
        [field: SerializeField] public ScrollRect ScrollRectUniversities { get; private set; }


        protected override void Awake()
        {
            base.Awake();
        }


        public override void Enable()
        {
            base.Enable();
            ShowUniversities();
            BackButton.onClick.AddListener(OnClickBackButton);
            RankingButton.onClick.AddListener(OnClickRankingButton);
            FilterInputField.onValueChanged.AddListener(delegate { OnFilterChanged(FilterInputField); });
        }

        public override void Disable()
        {
            base.Disable();
            BackButton.onClick.RemoveListener(OnClickBackButton);
            RankingButton.onClick.RemoveListener(OnClickRankingButton);
            FilterInputField.onValueChanged.RemoveAllListeners();
        }

        private void OnClickBackButton()
        {
            UIUFSelectionState uFSelectionState = UIStates.Instance.UFSelectionState;
            UIStateManager.Instance.StateMachine.TransitionTo(uFSelectionState);
        }

        private void OnFilterChanged(TMP_InputField filterInputField)
        {
            string filterText = filterInputField.text.Trim().ToLower();

            foreach (var uniButton in UniversityButtons)
            {
                string uniName = uniButton.University.GetCompleteName(removeAccents: true).ToLower();

                bool show = string.IsNullOrEmpty(filterText) || uniName.StartsWith(filterText) || uniName.Contains(filterText);
                uniButton.gameObject.SetActive(show);
            }
        }

        private void OnClickRankingButton()
        {
            LoadingPopUp.Instance?.Show();
            StartCoroutine(TransitionToNewState());
        }

        private IEnumerator TransitionToNewState()
        {
            yield return new WaitForSeconds(0.3f);
            UIRankingState rankingState = UIStates.Instance.RankingState;
            UIStateManager.Instance.StateMachine.TransitionTo(rankingState);
            rankingState.Manager.UIPanels.RankingUI.SetButtonText("Voltar");
            rankingState.Manager.UIPanels.RankingUI.BackButton.onClick.AddListener(() =>
            {
                UIUniversitySelectionState universitySelectionState = UIStates.Instance.UniversitySelectionState;
                UIStateManager.Instance.StateMachine.TransitionTo(universitySelectionState);
            });
        }


        public void ShowUniversities()
        {
            for (int i = UniversityButtons.Count - 1; i >= 0; i--)
            {
                Destroy(UniversityButtons[i].gameObject);
            }
            UniversityButtons.Clear();

            string UF = JsonDatabaseManager.Instance.UniversityDatabase.UF;
            Universities = JsonDatabaseManager.Instance.UniversityDatabase.GetUniversitiesByUF(UF);
            for (int i = 0; i < Universities.Count; i++)
            {
                UniversityDTO university = Universities[i];
                UniversityButton universitySelectButton = Instantiate(UniversityButtonPrefab, verticalContainer);
                universitySelectButton.Setup(university);
                UniversityButtons.Add(universitySelectButton);
            }

            StartCoroutine(SnapToTopWhenLayoutSettles(ScrollRectUniversities));
        }

        private IEnumerator SnapToTopWhenLayoutSettles(ScrollRect scrollRect)
        {
            var content = scrollRect.content;

            content.pivot = new Vector2(content.pivot.x, 1f);
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);

            scrollRect.StopMovement();
            var prevInertia = scrollRect.inertia;
            scrollRect.inertia = false;
            if (EventSystem.current) EventSystem.current.SetSelectedGameObject(null);

            float lastH = -1f;
            int safety = 12;
            while (safety-- > 0)
            {
                Canvas.ForceUpdateCanvases();
                LayoutRebuilder.ForceRebuildLayoutImmediate(content);

                float h = content.rect.height;
                if (Mathf.Approximately(h, lastH)) break;
                lastH = h;
                yield return null;
            }

            scrollRect.verticalNormalizedPosition = 1f;
            yield return new WaitForEndOfFrame();
            scrollRect.verticalNormalizedPosition = 1f;

            var pos = content.anchoredPosition;
            pos.y = 0f;
            content.anchoredPosition = pos;

            scrollRect.StopMovement();
            scrollRect.inertia = prevInertia;
        }
    }
}
