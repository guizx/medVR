using UnityEngine;
using UnityEngine.UI;

namespace Nato.UI
{
    public class ButtonSelectedSetting : MonoBehaviour
    {
        [field: SerializeField] public bool CanPlayButtonClickAudio = true;
        [field: SerializeField] public bool CanPlayButtonSelectedAudio = true;
        [field: SerializeField] public bool CanScaleButtonSelected = true;
        [field: SerializeField] public bool CanSelectUIFollow = true;
        [field: SerializeField] public bool CanSelectUIImage = false;
        [field: SerializeField] public bool CanChangeButtonSprites = false;
        [field: SerializeField] public bool SelectAnotherGameObject = false;
        [field: SerializeField] public bool CanChangeByColor = false;

        //[field: SerializeField] public SquashStretchTween SquashStretchTween;

        [field: SerializeField] public Vector3 ScaleButtonSelected = new Vector3(1.1f, 1.1f, 1f);
        [field: SerializeField] public Vector3 SelectUIOffset = Vector3.zero;
        [field: SerializeField] public GameObject SelectedImage;
        [field: SerializeField] public GameObject AnotherUI;
        [field: SerializeField] public Button CustomButton;
        [field: SerializeField] public Sprite NormalButtonSprite;
        [field: SerializeField] public Sprite SelectedButtonSprite;
        [field: SerializeField] public Color NormalButtonColor = Color.white;
        [field: SerializeField] public Color SelectedButtonColor = Color.white;



    }
}