using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MedGames
{
    public class UniversityRankingItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI rankingText;
        [SerializeField] private TextMeshProUGUI scoreText;

        [SerializeField] private Image medalImage;
        [SerializeField] private bool showMedal = true;
        [SerializeField] private Sprite[] medalSprites;


        public UniversityDTO University { get; private set; }

        public void Setup(int position, UniversityDTO university)
        {
            University = university;
            titleText.SetText(university.GetCompleteName(removeAccents: true));
            rankingText.SetText(position.ToString());
            scoreText.SetText(university.pontos.ToString("D2") + " pts");

            medalImage.gameObject.SetActive(showMedal);
            medalImage.sprite = medalSprites[position - 1];

        }
    }
}
