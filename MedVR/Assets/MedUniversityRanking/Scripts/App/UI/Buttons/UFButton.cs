using Nato.StateMachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MedGames
{
    public class UFButton : MonoBehaviour
    {
        [field: SerializeField] public Button Button { get; private set; }

        public TextMeshProUGUI UFText;
        public string UF;

        private void Awake()
        {
            Button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            Button.onClick.AddListener(OnClickButton);
        }

        private void OnDisable()
        {
            Button.onClick.RemoveListener(OnClickButton);
        }

        private void OnClickButton()
        {
            JsonDatabaseManager.Instance.UniversityDatabase.SetCurrentUF(UF);
            UIUniversitySelectionState universitySelectionState = UIStates.Instance.UniversitySelectionState;
            UIStateManager.Instance.StateMachine.TransitionTo(universitySelectionState);
        }

        public void Setup(string uf)
        {
            UFText.SetText(uf);
            UF = uf;
        }
    }
}
