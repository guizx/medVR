using System;
using DG.Tweening;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;
    [field: SerializeField] public CanvasGroup fadeCanvasGroup;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    public void FadeIn(float duration, Action callback)
    {
        fadeCanvasGroup.DOFade(endValue: 1f, duration: duration).OnComplete(() =>
        {
            callback?.Invoke();
        });
    }

    public void FadeOut(float duration, Action callback)
    {
        fadeCanvasGroup.DOFade(endValue: 0f, duration: duration).OnComplete(() =>
        {
            callback?.Invoke();
        });
    }
}
