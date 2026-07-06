using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI[] texts;
    [SerializeField] private Image[] images;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectColor = Color.yellow;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sfxHover;

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioSource.PlayOneShot(sfxHover);
        ChangeUIColors(selectColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ChangeUIColors(normalColor);
    }

    private void ChangeUIColors(Color newColor)
    {
        foreach (var text in texts)
            text.color = newColor;
        foreach (var image in images)
            image.color = newColor;
    }
}
