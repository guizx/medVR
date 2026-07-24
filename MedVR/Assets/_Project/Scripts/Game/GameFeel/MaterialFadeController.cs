using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class MaterialFadeController : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private float defaultDuration = 1f;

    [Header("Unity Events - Fade In")]
    [SerializeField] private UnityEvent onFadeInStart;
    [SerializeField] private UnityEvent onFadeInComplete;

    [Header("Unity Events - Fade Out")]
    [SerializeField] private UnityEvent onFadeOutStart;
    [SerializeField] private UnityEvent onFadeOutComplete;

    private MaterialPropertyBlock propertyBlock;
    private static readonly int BaseColorProperty = Shader.PropertyToID("_BaseColor");
    private Color currentColor = Color.white;
    private Tween currentTween;

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();

        if (targetRenderer != null)
        {
            targetRenderer.GetPropertyBlock(propertyBlock);
            currentColor = targetRenderer.sharedMaterial.HasProperty(BaseColorProperty)
                ? targetRenderer.sharedMaterial.GetColor(BaseColorProperty)
                : Color.white;

            propertyBlock.SetColor(BaseColorProperty, currentColor);
            targetRenderer.SetPropertyBlock(propertyBlock);
        }
    }

    public void FadeIn(float duration = -1f)
    {
        if (targetRenderer == null) return;

        float dur = duration < 0 ? defaultDuration : duration;
        onFadeInStart?.Invoke();

        currentTween?.Kill();
        currentTween = DOTween.To(() => currentColor.a, x =>
        {
            currentColor.a = x;
            targetRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(BaseColorProperty, currentColor);
            targetRenderer.SetPropertyBlock(propertyBlock);
        }, 1f, dur).OnComplete(() => onFadeInComplete?.Invoke());
    }

    public void FadeOut(float duration = -1f)
    {
        if (targetRenderer == null) return;

        float dur = duration < 0 ? defaultDuration : duration;
        onFadeOutStart?.Invoke();

        currentTween?.Kill();
        currentTween = DOTween.To(() => currentColor.a, x =>
        {
            currentColor.a = x;
            targetRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(BaseColorProperty, currentColor);
            targetRenderer.SetPropertyBlock(propertyBlock);
        }, 0f, dur).OnComplete(() => onFadeOutComplete?.Invoke());
    }

    [ContextMenu("FadeInAndFadeOut")]
    public void FadeInAndFadeOut(float holdDuration = 1f, float duration = -1f)
    {
        if (targetRenderer == null) return;

        float dur = duration < 0 ? defaultDuration : duration;

        currentTween?.Kill();

        Sequence sequence = DOTween.Sequence();

        sequence.AppendCallback(() => onFadeInStart?.Invoke());
        sequence.Append(DOTween.To(() => currentColor.a, x =>
        {
            currentColor.a = x;
            targetRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(BaseColorProperty, currentColor);
            targetRenderer.SetPropertyBlock(propertyBlock);
        }, 1f, dur));
        sequence.AppendCallback(() => onFadeInComplete?.Invoke());

        sequence.AppendInterval(holdDuration);

        sequence.AppendCallback(() => onFadeOutStart?.Invoke());
        sequence.Append(DOTween.To(() => currentColor.a, x =>
        {
            currentColor.a = x;
            targetRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(BaseColorProperty, currentColor);
            targetRenderer.SetPropertyBlock(propertyBlock);
        }, 0f, dur));
        sequence.AppendCallback(() => onFadeOutComplete?.Invoke());

        currentTween = sequence;
    }

    [ContextMenu("FadeIn")]
    public void EditorFadeIn()
    {
        FadeIn();
    }

    [ContextMenu("FadeOut")]
    public void EditorFadeOut()
    {
        FadeOut();
    }

    [ContextMenu("FadeInAndFadeOut")]
    public void EditorFadeInAndFadeOut()
    {
        FadeInAndFadeOut();
    }
}
