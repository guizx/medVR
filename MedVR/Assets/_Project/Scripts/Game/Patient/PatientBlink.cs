using System.Collections;
using UnityEngine;

public class PatientBlink : MonoBehaviour
{
    [SerializeField] private Animator patientAnimator;

    [Header("Configurações")]
    [SerializeField] private float minInterval = 2.0f;
    [SerializeField] private float maxInterval = 5.0f;
    [SerializeField] private float blinkSpeed = 10f;

    private void Start()
    {
        StartCoroutine(BlinkRoutine());
    }

    private IEnumerator BlinkRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));

            yield return StartCoroutine(LerpBlink(0f, 1f));

            yield return new WaitForSeconds(0.05f);

            yield return StartCoroutine(LerpBlink(1f, 0f));
        }
    }

    private IEnumerator LerpBlink(float startValue, float endValue)
    {
        float elapsed = 0f;
        float duration = 1f / blinkSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            float currentValue = Mathf.Lerp(startValue, endValue, t);

            patientAnimator.SetFloat("eyeBlinkLeft", currentValue);
            patientAnimator.SetFloat("eyeBlinkRight", currentValue);

            yield return null;
        }

        patientAnimator.SetFloat("eyeBlinkLeft", endValue);
        patientAnimator.SetFloat("eyeBlinkRight", endValue);
    }
}
