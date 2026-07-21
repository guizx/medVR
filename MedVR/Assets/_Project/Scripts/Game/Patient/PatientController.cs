using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;

public class PatientController : MonoBehaviour
{
    [Header("Data")]
    public PatientData currentPatient;
    public DiseaseData currentDisease;

    [Header("Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator patientAnimator;
    public float maxMouthOpen = 1f;
    public float mouthSpeedLerp = 10f;
    private Coroutine speechRoutine;

    public UnityEvent OnConsultationStarted;

    public void Setup(PatientData patientData, DiseaseData diseaseData)
    {
        currentPatient = patientData;
        currentDisease = diseaseData;
    }

    private void OnDisable()
    {
        if (speechRoutine != null)
        {
            StopCoroutine(speechRoutine);
            speechRoutine = null;
        }

        audioSource.Stop();
        patientAnimator.SetFloat("ih", 0);
    }

    public void StartConsultation()
    {
        if (!GameManager.Instance.OnConsultation)
            GameManager.Instance.ComputerUI.BeginConsultation(this);

        GetComponent<BoxCollider>().enabled = false;
        OnConsultationStarted?.Invoke();
    }

    public void Speak(int phraseIndex, Action onFinished)
    {
        if (speechRoutine != null)
            StopCoroutine(speechRoutine);

        speechRoutine = StartCoroutine(PlayPhraseRoutine(
            currentDisease.Phrases[phraseIndex],
            onFinished));
    }

    private IEnumerator PlayPhraseRoutine(PatientPhrase data, Action onFinished)
    {
        if (data.PhraseSFX != null)
        {
            audioSource.clip = data.PhraseSFX;
            audioSource.Play();
        }

        float duration = data.PhraseSFX != null ? data.PhraseSFX.length : 2.0f;
        GameManager.Instance.ShowSubtitle(data.Phrase, duration);

        float timer = 0f;
        float[] spectrum = new float[256];
        float currentMouthValue = 0f;

        while (timer < duration)
        {
            if (data.PhraseSFX != null)
            {
                audioSource.GetOutputData(spectrum, 0);
                float peak = 0f;
                foreach (float s in spectrum) peak = Mathf.Max(peak, Mathf.Abs(s));

                float targetValue = Mathf.Clamp(peak * mouthSpeedLerp, 0f, maxMouthOpen);
                currentMouthValue = Mathf.Lerp(currentMouthValue, targetValue, Time.deltaTime * mouthSpeedLerp);
                patientAnimator.SetFloat("ih", currentMouthValue);
            }
            timer += Time.deltaTime;
            yield return null;
        }

        patientAnimator.SetFloat("ih", 0f);
        onFinished?.Invoke();
    }
}