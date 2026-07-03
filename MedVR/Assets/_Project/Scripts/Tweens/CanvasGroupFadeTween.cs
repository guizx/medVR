using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupFadeTween : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    [Header("Settings")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private bool autoFadeIn = false;
    [SerializeField] private bool autoFadeOut = false;

    [Header("Events")]
    public UnityEvent OnFadeInStart;
    public UnityEvent OnFadeInEnded;
    public UnityEvent OnFadeOutStart;
    public UnityEvent OnFadeOutEnded;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        if (autoFadeIn) FadeIn();
        else if (autoFadeOut) FadeOut();
    }

    public void FadeIn()
    {
        OnFadeInStart?.Invoke();
        
        canvasGroup.DOFade(1f, fadeInDuration)
            .OnComplete(() => OnFadeInEnded?.Invoke());
    }

    public void FadeOut()
    {
        OnFadeOutStart?.Invoke();
        
        canvasGroup.DOFade(0f, fadeOutDuration)
            .OnComplete(() => OnFadeOutEnded?.Invoke());
    }
}