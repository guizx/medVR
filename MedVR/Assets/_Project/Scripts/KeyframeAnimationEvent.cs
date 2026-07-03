using UnityEngine;
using UnityEngine.Events;

public class KeyframeAnimationEvent : MonoBehaviour
{
    public UnityEvent OnKeyframeInvoked;

    public void InvokeEvent()
    {
        OnKeyframeInvoked?.Invoke();
    }
}
