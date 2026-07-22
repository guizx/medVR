using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MedGames
{
    public class AnswerButton : MonoBehaviour
    {
        [field: SerializeField] public Button Button { get; private set; }
        [field: SerializeField] public TextMeshProUGUI AnswerText { get; private set; }
        [field: SerializeField] public TextMeshProUGUI AlternativeText { get; private set; }

        private void Awake()
        {
            Button = GetComponent<Button>();
        }


        public void Setup(string answer, int index)
        {
            AnswerText.SetText(answer);
            switch(index) {
                case 0:
                    AlternativeText.SetText("A:");
                    break;
                case 1:
                    AlternativeText.SetText("B:");
                    break;
                case 2:
                    AlternativeText.SetText("C:");
                    break;
                case 3:
                    AlternativeText.SetText("D:");
                    break;
            }
        }
    }
}
