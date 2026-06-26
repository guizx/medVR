using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class DiegeticButton : MonoBehaviour
{
[SerializeField] private float pressAmount = 0.05f;
    [SerializeField] private float duration = 0.1f;
    [SerializeField] private UnityEvent onButtonPressed;

    private Vector3 initialPosition;
    private bool isAnimating = false;

    private void Start()
    {
        initialPosition = transform.localPosition;
    }

    public void Press()
    {
        if (isAnimating) return;

        isAnimating = true;
        
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOLocalMoveY(initialPosition.y - pressAmount, duration));
        s.Append(transform.DOLocalMoveY(initialPosition.y, duration));
        s.OnComplete(() => isAnimating = false);

        onButtonPressed?.Invoke();
    }
}
