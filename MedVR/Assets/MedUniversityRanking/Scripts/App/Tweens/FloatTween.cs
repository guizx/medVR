using DG.Tweening;
using UnityEngine;

namespace Nato.GameFeel.Tween
{
    public class FloatTween : MonoBehaviour
    {
        [SerializeField] private float offsetY = 0.15f;
        [SerializeField] private float offsetX = 0f;
        [SerializeField] private float durationLoop = 0.3f;
        [SerializeField] private Ease ease = Ease.Linear;

        [SerializeField] private bool isHorizontal;
        private void Start()
        {
            if(!isHorizontal)
                transform.DOLocalMoveY(transform.localPosition.y + offsetY, durationLoop).SetEase(ease).SetLoops(-1, LoopType.Yoyo);
            else
                transform.DOLocalMoveX(transform.localPosition.x + offsetX, durationLoop).SetEase(ease).SetLoops(-1, LoopType.Yoyo);


        }
    }
}