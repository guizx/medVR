using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComputerUI : MonoBehaviour
{
    public TextMeshProUGUI PatientNameText;
    public TextMeshProUGUI PatientAgeText;
    public TextMeshProUGUI PatientGenreText;
    public TextMeshProUGUI PatientTriageReportText;
    public TextMeshProUGUI PatientSymptomText;
    public TextMeshProUGUI PatientEmergencyCode;
    public TextMeshProUGUI SymptomTitleText;

    public Button SymptomButtonPrefab;
    public Transform SymptomContainer;
    private List<Button> spawnedButtons = new List<Button>();

    [SerializeField] private DiseaseList allPossibleDiseases;

    private PatientController activePatient;
    private int currentPhraseIndex = 0;

    public GameObject EndingConsultationPanel;
    public TextMeshProUGUI ConsultationText;

    public AudioSource audioSource;
    public AudioClip SFX_Success;
    public AudioClip SFX_Error;
    public GameObject ComputerPanel;


    public void Setup(PatientData patient, DiseaseData disease)
    {
        PatientNameText.text = $"Nome: {patient.Name}";
        PatientAgeText.text = $"Idade: {patient.Age}";
        PatientGenreText.text = $"Gênero: {patient.Genre}";
        PatientTriageReportText.text = $"Triagem: {disease.DiseaseTriage[Random.Range(0, disease.DiseaseTriage.Length)]}";
        PatientEmergencyCode.text = $"Código: {disease.EmergencyCode}";
        EndingConsultationPanel.SetActive(false);
        ComputerPanel.SetActive(true);
    }

    public void ClearAllText()
    {
        PatientNameText.text = "";
        PatientAgeText.text = "";
        PatientGenreText.text = "";
        PatientTriageReportText.text = "";
        PatientSymptomText.text = "Sintomas";
        PatientEmergencyCode.text = "";
    }

    public void BeginConsultation(PatientController patient)
    {
        GameManager.Instance.OnConsultation = true;
        activePatient = patient;
        currentPhraseIndex = 0;

        PatientNameText.text = $"Nome: {patient.currentPatient.Name}";
        PatientAgeText.text = $"Idade: {patient.currentPatient.Age}";
        PatientGenreText.text = $"Gênero: {patient.currentPatient.Genre}";
        PatientTriageReportText.text = $"Triagem: {patient.currentDisease.DiseaseTriage[Random.Range(0, patient.currentDisease.DiseaseTriage.Length)]}";
        PatientEmergencyCode.text = $"Código: {patient.currentDisease.EmergencyCode}";

        ClearAllSymptoms();
        PlayNextStep();
    }

    private void PlayNextStep()
    {
        ClearAllButtons();
        if (currentPhraseIndex < activePatient.currentDisease.Phrases.Count)
        {
            SymptomTitleText.text = "Ouvindo paciente...";
            activePatient.Speak(currentPhraseIndex, () => ShowOptions());
        }
        else
        {
            ShowDiagnosisPhase();
        }
    }

    private void ShowOptions()
    {
        SymptomTitleText.text = "Qual o sintoma?";
        var options = activePatient.currentDisease.Phrases[currentPhraseIndex].Options;
        foreach (var opt in options)
        {
            Button btn = Instantiate(SymptomButtonPrefab, SymptomContainer);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = opt.SymptomName;
            btn.onClick.AddListener(() => StartCoroutine(HandleSelection(btn, opt)));
            spawnedButtons.Add(btn);
        }
    }

    private IEnumerator HandleSelection(Button btn, SymptomOption opt)
    {
        SetButtonsInteractable(false);

        btn.GetComponent<Image>().color = opt.IsCorrect ? Color.green : Color.red;

        if (opt.IsCorrect)
        {
            audioSource.PlayOneShot(SFX_Success);

            AddSymptom(opt.SymptomName);

            yield return new WaitForSeconds(0.5f);

            currentPhraseIndex++;
            PlayNextStep();
        }
        else
        {
            audioSource.PlayOneShot(SFX_Error);

            yield return new WaitForSeconds(0.5f);

            btn.GetComponent<Image>().color = Color.white;

            SetButtonsInteractable(true);
        }
    }

    private void ShowDiagnosisPhase()
    {
        SymptomTitleText.text = "Qual é o seu diagnóstico?";
        ClearAllButtons();

        List<DiseaseData> options = GetRandomDiseases(3);

        foreach (var disease in options)
        {
            Button btn = Instantiate(SymptomButtonPrefab, SymptomContainer);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = disease.DiseaseName;

            btn.onClick.AddListener(() => StartCoroutine(HandleDiagnosisSelection(btn, disease == activePatient.currentDisease)));
            spawnedButtons.Add(btn);
        }
    }

    private IEnumerator HandleDiagnosisSelection(Button btn, bool isCorrect)
    {
        SetButtonsInteractable(false);

        btn.GetComponent<Image>().color = isCorrect ? Color.green : Color.red;

        yield return new WaitForSeconds(1.0f);

        if (isCorrect)
        {
            audioSource.PlayOneShot(SFX_Success);
            EndingConsultationPanel.SetActive(true);
            ConsultationText.text = "Diagnóstico Correto! O paciente agradece.";

            ClearAllButtons();
            ClearAllText();
            activePatient.enabled = false;
            GameManager.Instance.OnConsultation = false;
        }
        else
        {
            audioSource.PlayOneShot(SFX_Error);

            btn.GetComponent<Image>().color = Color.white;
            SetButtonsInteractable(true);
        }
    }

    private void SetButtonsInteractable(bool isInteractable)
    {
        foreach (Button btn in spawnedButtons)
        {
            if (btn != null)
            {
                btn.interactable = isInteractable;
            }
        }
    }

    private List<DiseaseData> GetRandomDiseases(int count)
    {
        List<DiseaseData> result = new List<DiseaseData> { activePatient.currentDisease };

        var others = new List<DiseaseData>(allPossibleDiseases.AllDiseases);
        others.Remove(activePatient.currentDisease);

        for (int i = 0; i < others.Count; i++)
        {
            int rnd = Random.Range(i, others.Count);
            DiseaseData temp = others[i];
            others[i] = others[rnd];
            others[rnd] = temp;
        }

        while (result.Count < count && others.Count > 0)
        {
            result.Add(others[0]);
            others.RemoveAt(0);
        }

        for (int i = 0; i < result.Count; i++)
        {
            int rnd = Random.Range(i, result.Count);
            DiseaseData temp = result[i];
            result[i] = result[rnd];
            result[rnd] = temp;
        }

        return result;
    }

    private void FinalDiagnosis(bool isCorrect)
    {
        EndingConsultationPanel.SetActive(true);
        ConsultationText.text = isCorrect ? "Diagnóstico Correto! O paciente agradece." : "Diagnóstico Incorreto. Tente novamente.";
        ClearAllButtons();
        ClearAllText();
        GameManager.Instance.OnConsultation = false;

    }

    public void AddSymptom(string symptom) => PatientSymptomText.text += (PatientSymptomText.text == "" ? "" : ", ") + symptom;
    public void ClearAllSymptoms() => PatientSymptomText.text = "Sintomas:";
    private void ClearAllButtons()
    {
        foreach (var b in spawnedButtons)
        {
            b.onClick.RemoveAllListeners();
            Destroy(b.gameObject);
        }
        spawnedButtons.Clear();
    }
}
