using DG.Tweening;
using UnityEngine;

public class FloatTwen : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 initialAnchoredPosition;


    [SerializeField] private Vector2 punchOffset = new Vector2(0f, 20f);

    [SerializeField] private float duration = 1f;

    [SerializeField] private LoopType loopType = LoopType.Yoyo;

    [SerializeField] private Ease easeType = Ease.InOutSine;

    private Tween floatTween;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            initialAnchoredPosition = rectTransform.anchoredPosition;
        }
    }

    private void OnEnable()
    {
        StartFloatLoop();
    }

    private void OnDisable()
    {
        floatTween?.Kill();
    }

    public void StartFloatLoop()
    {
        if (rectTransform == null) return;

        floatTween?.Kill();

        rectTransform.anchoredPosition = initialAnchoredPosition;

        Vector2 targetPosition = initialAnchoredPosition + punchOffset;

        floatTween = rectTransform.DOAnchorPos(targetPosition, duration)
            .SetEase(easeType)
            .SetLoops(-1, loopType)
            .SetUpdate(true);
    }
}
