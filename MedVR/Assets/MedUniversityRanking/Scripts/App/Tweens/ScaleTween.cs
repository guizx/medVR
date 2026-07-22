using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Nato.GameFeel.Tween
{
    public class ScaleTween : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float delay = 0f;

        [SerializeField] private float duration = 0.15f;
        [SerializeField] private Vector2 endValue;
        [SerializeField] private bool loopStart;

        [SerializeField] private bool byEnable;
        [SerializeField] private bool zeroDisable;
        [SerializeField] private bool zeroEnable;

        public UnityEvent OnEnableTween;

        public UnityEvent OnEndTween;

        [SerializeField] private bool canShake;

        private void OnEnable()
        {
            if (zeroEnable)
                target.localScale = Vector3.zero;

            if (byEnable)
                PlayTween();
        }

        private void OnDisable()
        {
            if (zeroDisable)
                transform.localScale = Vector3.zero;
        }

        private void Start()
        {
            if (loopStart)
            {
                target.DOScale(endValue, duration).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            }
        }

        public void PlayTween()
        {
            OnEnableTween?.Invoke();
            Sequence sequence = DOTween.Sequence();
            sequence.Append(target.DOScale(endValue, duration).OnComplete(() =>
            {
            }).SetDelay(delay));

            if(canShake)
                sequence.Append(target.DOShakeScale(duration: 0.15f, 0.5f));
            sequence.OnComplete(() =>
            {
                transform.localScale = Vector3.one;
                OnEndTween?.Invoke();
            });
            
        }
    }
}