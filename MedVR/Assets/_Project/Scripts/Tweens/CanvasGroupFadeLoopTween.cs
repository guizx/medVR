using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupFadeLoopTween : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    [SerializeField] private float duration = 1f;
    [SerializeField] private float minAlpha = 0f;
    [SerializeField] private float maxAlpha = 1f;
    [SerializeField] private LoopType loopType = LoopType.Yoyo;

    private Tween fadeTween;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        StartFadeLoop();
    }

    private void OnDisable()
    {
        fadeTween?.Kill();
    }
    public void StartFadeLoop()
    {
        fadeTween?.Kill();

        canvasGroup.alpha = minAlpha;

        fadeTween = canvasGroup.DOFade(maxAlpha, duration)
            .SetLoops(-1, loopType)
            .SetUpdate(true);
    }
}
