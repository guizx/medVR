using Nato.StateMachine;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MedGames
{
    [RequireComponent(typeof(Button))]
    public class UniversityButton : MonoBehaviour
    {
        public Button Button { get; private set; }
        [SerializeField] private TextMeshProUGUI titleText;

        public UniversityDTO University { get; private set; }

        private void Awake()
        {
            Button = GetComponent<Button>();
            Button.onClick.AddListener(OnClickButton);
        }

        private void OnClickButton()
        {
            JsonDatabaseManager.Instance.UniversityDatabase.SetUniversitySelected(University);
            UIMainScreenState mainScreenState = UIStates.Instance.MainScreenState;
            UIStateManager.Instance.StateMachine.TransitionTo(mainScreenState);
            GameManager.Instance.InitializeCutscene();
            GameManager.Instance.SetCurrentUniversity(University);
        }

        private void OnDestroy()
        {
            if (Button != null)
                Button.onClick.RemoveAllListeners();
        }

        public void Setup(UniversityDTO university)
        {
            titleText.SetText(university.GetCompleteName(removeAccents: true));
            University = university;
        }
    }
}
