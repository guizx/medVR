using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MedGames
{
    public class ResetRankingButton : MonoBehaviour
    {
        public Button Button { get; private set; }

        private void Awake()
        {
            Button = GetComponent<Button>();
            Button.onClick.AddListener(OnClickResetRankingButton);
        }

        private void OnDestroy()
        {
            Button.onClick.RemoveListener(OnClickResetRankingButton);
        }

        private void OnClickResetRankingButton()
        {
            UniversityDatabase database = FindFirstObjectByType<UniversityDatabase>();
            if(database != null)
            {
                database.ResetAllScores();
                SceneManager.LoadScene(0);
            }
        }
    }
}
