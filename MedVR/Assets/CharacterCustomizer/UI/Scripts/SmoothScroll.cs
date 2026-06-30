using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace CC
{
    public class SmoothScroll : ScrollRect
    {
        public bool SmoothScrolling { get; set; } = true;
        public float SmoothScrollTime { get; set; } = 0.2f;

        private Coroutine smoothScrollCoroutine;

        public override void OnScroll(PointerEventData data)
        {
            if (!IsActive()) return;

            if (SmoothScrolling)
            {
                // Stop any ongoing smooth scroll
                if (smoothScrollCoroutine != null)
                    StopCoroutine(smoothScrollCoroutine);

                Vector2 positionBefore = normalizedPosition;
                base.OnScroll(data);
                Vector2 positionAfter = normalizedPosition;
                normalizedPosition = positionBefore;

                smoothScrollCoroutine = StartCoroutine(SmoothScrollToPosition(positionAfter));
            }
            else
            {
                base.OnScroll(data);
            }
        }

        public void SetScrollTarget(Vector2 targetPosition)
        {
            if (SmoothScrolling)
            {
                if (smoothScrollCoroutine != null)
                    StopCoroutine(smoothScrollCoroutine);

                Vector2 positionBefore = normalizedPosition;
                normalizedPosition = targetPosition;
                Vector2 positionAfter = normalizedPosition;
                normalizedPosition = positionBefore;

                smoothScrollCoroutine = StartCoroutine(SmoothScrollToPosition(positionAfter));
            }
            else
            {
                normalizedPosition = targetPosition;
            }
        }

        public void resetScroll(bool instant = false)
        {
            var smoothScroll = SmoothScrolling;
            if (instant) SmoothScrolling = false;
            StopAllCoroutines();
            SetScrollTarget(new Vector2(0, 1));
            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
            if (instant) SmoothScrolling = smoothScroll;
        }

        public void ScrollToContent(RectTransform targetContent)
        {
            // Ensure targetContent is a child of the content area
            if (targetContent == null || !targetContent.IsChildOf(content))
            {
                Debug.LogWarning("Target content is not a child of the scroll rect content.");
                return;
            }

            // Calculate the target normalized position
            Vector2 targetLocalPos = content.InverseTransformPoint(targetContent.position);
            Vector2 contentSize = content.rect.size;
            Vector2 viewportSize = viewport.rect.size;

            Vector2 normalizedPos = new Vector2(
                Mathf.Clamp01((targetLocalPos.x - viewportSize.x / 2) / (contentSize.x - viewportSize.x)),
                Mathf.Clamp01((targetLocalPos.y - viewportSize.y / 2) / (contentSize.y - viewportSize.y))
            );

            SetScrollTarget(normalizedPos);
        }

        private IEnumerator SmoothScrollToPosition(Vector2 targetPosition)
        {
            float elapsedTime = 0f;
            Vector2 startPosition = normalizedPosition;

            while (elapsedTime < SmoothScrollTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                normalizedPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / SmoothScrollTime);
                yield return null;
            }

            normalizedPosition = targetPosition;
            smoothScrollCoroutine = null;
        }
    }
}