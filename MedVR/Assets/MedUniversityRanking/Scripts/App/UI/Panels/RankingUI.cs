using Nato.StateMachine;
using Nato.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MedGames
{
    public class RankingUI : BaseUI
    {
        [field: Header("Buttons")]
        [field: SerializeField] public Button BackButton { get; private set; }

        [Header("Containers")]
        [SerializeField] private Transform verticalContainer;

        [Header("Prefabs")]
        [field: SerializeField] public UniversityRankingItem UniversityRankingItemPrefab { get; private set; }

        [Header("Lists")]
        private List<UniversityRankingItem> rankings = new List<UniversityRankingItem>();

        [Header("UI Elements")]
        [field: SerializeField] public TMP_InputField FilterInputField { get; private set; }
        [field: SerializeField] public ScrollRect ScrollRectUniversities { get; private set; }


        protected override void Awake()
        {
            base.Awake();
        }

        public override void Enable()
        {
            base.Enable();
            LoadingPopUp.Instance.Hide();
            ShowRankings();
            FilterInputField.onValueChanged.AddListener(delegate { OnFilterChanged(FilterInputField); });
        }

        public override void Disable()
        {
            base.Disable();

            BackButton.onClick.RemoveAllListeners();
            FilterInputField.onValueChanged.RemoveAllListeners();

        }


        private void OnFilterChanged(TMP_InputField filterInputField)
        {
            string filterText = filterInputField.text.Trim().ToLower();

            foreach (var uniButton in rankings)
            {
                string uniName = uniButton.University.GetCompleteName(removeAccents: true).ToLower();

                bool show = string.IsNullOrEmpty(filterText) || uniName.StartsWith(filterText) || uniName.Contains(filterText);
                uniButton.gameObject.SetActive(show);
            }
        }

        public void SetButtonText(string text)
        {
            BackButton.GetComponentInChildren<TextMeshProUGUI>().text = text;
        }

        private void OnClickBackButton()
        {
            UIMainScreenState mainScreenState = UIStates.Instance.MainScreenState;
            UIStateManager.Instance.StateMachine.TransitionTo(mainScreenState);
        }

        private void DestroyRankings()
        {
            for (int i = rankings.Count - 1; i >= 0; i--)
            {
                Destroy(rankings[i].gameObject);
            }
            rankings.Clear();
        }

        public void ShowRankings()
        {
            DestroyRankings();

            List<UniversityDTO> orderedRankings = JsonDatabaseManager.Instance.UniversityDatabase.Universities
                .OrderByDescending(r => r.pontos)
                .ToList();

            int i = 1;
            foreach (UniversityDTO university in orderedRankings)
            {
                UniversityRankingItem rankingItem = Instantiate(UniversityRankingItemPrefab, verticalContainer);
                rankingItem.Setup(i, university);
                rankings.Add(rankingItem);
                i++;

                if (i > 3)
                    break;
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
