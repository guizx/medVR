using Nato.StateMachine;
using Nato.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MedGames
{
    public class UniversityCreateUI : BaseUI
    {
        [field: Header("Input Fields")]
        [field: SerializeField] public TMP_InputField IESInputField { get; private set; }
        [field: SerializeField] public TMP_InputField CampusInputField { get; private set; }

        [field: Header("Dropdowns")]
        [field: SerializeField] public TMP_Dropdown UFDropdown { get; private set; }

        [field: Header("Buttons")]
        [field: SerializeField] public Button AddButton { get; private set; }
        [field: SerializeField] public Button BackButton { get; private set; }


        private void OnEnable()
        {
            AddButton.onClick.AddListener(OnClickAddButton);
            BackButton.onClick.AddListener(OnClickBackButton);
        }

        private void OnDisable()
        {
            AddButton.onClick.RemoveListener(OnClickAddButton);
            BackButton.onClick.RemoveListener(OnClickBackButton);
        }

        private void OnClickAddButton()
        {
            if (string.IsNullOrEmpty(IESInputField.text) ||
                string.IsNullOrEmpty(CampusInputField.text))
                return;


            string id = JsonDatabaseManager.Instance.UniversityDatabase.Universities.Count.ToString();
            string ies = IESInputField.text;
            string campus = CampusInputField.text;
            string uf = UFDropdown.options[UFDropdown.value].text.Substring(0, 2);

            UniversityDTO universityDTO = new UniversityDTO();
            universityDTO.id = id;
            universityDTO.ies = ies;
            universityDTO.campus = campus;
            universityDTO.uf = uf;
            universityDTO.pontos = 0;
            universityDTO.municipio = "";


            JsonDatabaseManager.Instance.UniversityDatabase.AddUniversity(universityDTO);
            UITitleScreenState titleScreenState = UIStates.Instance.TitleScreenState;
            UIStateManager.Instance.StateMachine.TransitionTo(titleScreenState);

        }

        private void OnClickBackButton()
        {
            UITitleScreenState titleScreenState = UIStates.Instance.TitleScreenState;
            UIStateManager.Instance.StateMachine.TransitionTo(titleScreenState);
        }
    }
}
