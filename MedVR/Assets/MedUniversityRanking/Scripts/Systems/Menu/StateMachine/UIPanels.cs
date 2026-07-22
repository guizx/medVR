using MedGames;
using UnityEngine;

namespace Nato.StateMachine
{
    public class UIPanels : MonoBehaviour
    {
        [field: Header("Global UI")]
        [field: SerializeField] public UFSelectionUI UFSelectionUI { get; private set; }
        [field: SerializeField] public TitleScreenUI TitleScreenUI { get; private set; }
        [field: SerializeField] public UniversitySelectionUI UniversitySelectionUI { get; private set; }
        [field: SerializeField] public MainScreenUI MainScreenUI { get; private set; }
        [field: SerializeField] public RankingUI RankingUI { get; private set; }
        [field: SerializeField] public UniversityCreateUI UniversityCreateUI { get; private set; }
    }
}
