using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PatientController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private DiseaseData currentDisease;
    [SerializeField] private DiseaseList allPossibleDiseases;

    [Header("UI Components")]
    [SerializeField] private GameObject dialogueContainer;
    [SerializeField] private TextMeshProUGUI phraseText;
    [SerializeField] private TextMeshProUGUI symptomListText;
    [SerializeField] private Transform buttonsContainer;
    [SerializeField] private Button buttonPrefab;

    [Header("Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float delayBetweenPhrases = 1.5f;

    private List<Button> spawnedButtons = new List<Button>();
    private List<string> collectedSymptoms = new List<string>();
    private int currentPhraseIndex = 0;

    private void Start()
    {
        if (dialogueContainer != null) dialogueContainer.SetActive(false);
        if (symptomListText != null) symptomListText.text = "";
    }

    public void StartConsultation()
    {
        if (dialogueContainer == null || dialogueContainer.activeSelf) return;

        dialogueContainer.SetActive(true);
        currentPhraseIndex = 0;
        collectedSymptoms.Clear();
        symptomListText.text = "";
        LoadPhrase();
    }

    private void LoadPhrase()
    {
        ClearButtons();

        if (currentPhraseIndex >= currentDisease.Phrases.Count)
        {
            ShowDiagnosisPhase();
            return;
        }

        var data = currentDisease.Phrases[currentPhraseIndex];
        StartCoroutine(PlayPhraseRoutine(data));
    }

    private IEnumerator PlayPhraseRoutine(PatientPhrase data)
    {
        phraseText.text = "";
        if (data.PhraseSFX != null) audioSource.PlayOneShot(data.PhraseSFX);

        float duration = data.PhraseSFX != null ? data.PhraseSFX.length : 2.0f;
        float charDelay = (duration - 0.5f) / Mathf.Max(data.Phrase.Length, 1);

        foreach (char c in data.Phrase)
        {
            phraseText.text += c;
            yield return new WaitForSeconds(charDelay);
        }

        yield return new WaitForSeconds(0.5f);
        SpawnSymptomButtons(data.Options);
    }

    private void SpawnSymptomButtons(SymptomOption[] options)
    {
        symptomListText.text = "Qual o sintoma?";
        foreach (var opt in options)
        {
            Button btn = Instantiate(buttonPrefab, buttonsContainer);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = opt.SymptomName;
            spawnedButtons.Add(btn);
            btn.onClick.AddListener(() => StartCoroutine(HandleSymptomSelection(btn, opt)));
        }
    }

    private IEnumerator HandleSymptomSelection(Button clickedButton, SymptomOption option)
    {
        SetButtonsInteractable(false);
        Image btnImage = clickedButton.GetComponent<Image>();
        btnImage.color = option.IsCorrect ? Color.green : Color.red;

        if (option.IsCorrect)
        {
            collectedSymptoms.Add(option.SymptomName);
            yield return new WaitForSeconds(delayBetweenPhrases);
            symptomListText.text = "";

            currentPhraseIndex++;
            LoadPhrase();
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            SetButtonsInteractable(true);
            btnImage.color = Color.white;
        }
    }

    private void ShowDiagnosisPhase()
    {
        dialogueContainer.SetActive(false);
        symptomListText.text = $"Sintomas observados: <color=yellow>{string.Join(", ", collectedSymptoms)}</color>\n\nQual é o seu diagnóstico?";

        ClearButtons();

        List<DiseaseData> options = GetRandomDiseases(3);

        foreach (var disease in options)
        {
            Button btn = Instantiate(buttonPrefab, buttonsContainer);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = disease.DiseaseName;
            btn.onClick.AddListener(() => FinalDiagnosis(disease == currentDisease));
            spawnedButtons.Add(btn);
        }
    }

    private List<DiseaseData> GetRandomDiseases(int count)
    {
        List<DiseaseData> result = new List<DiseaseData> { currentDisease };
        var others = allPossibleDiseases.AllDiseases.FindAll(d => d != currentDisease);

        for (int i = 0; i < others.Count; i++)
        {
            int rnd = Random.Range(i, others.Count);
            var temp = others[i]; others[i] = others[rnd]; others[rnd] = temp;
        }

        while (result.Count < count && others.Count > 0)
        {
            result.Add(others[0]);
            others.RemoveAt(0);
        }
        return result;
    }

    private void FinalDiagnosis(bool isCorrect)
    {
        symptomListText.text = isCorrect ? "Correto! O paciente agradece." : "Diagnóstico incorreto. Tente novamente.";
        ClearButtons();
        StartCoroutine(CloseConsultation());
    }

    private IEnumerator CloseConsultation()
    {
        yield return new WaitForSeconds(2.0f);
        dialogueContainer.SetActive(false);
    }

    private void SetButtonsInteractable(bool state)
    {
        foreach (var b in spawnedButtons) if (b != null) b.interactable = state;
    }

    private void ClearButtons()
    {
        foreach (var btn in spawnedButtons) if (btn != null) Destroy(btn.gameObject);
        spawnedButtons.Clear();
    }
}