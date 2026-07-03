using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private GameObject patientModel;
    [SerializeField] private GameObject computerInfo;
    [SerializeField] private FirstPersonCamera doctorCamera;
    [SerializeField] private Transform computer;
    [SerializeField] private TextMeshProUGUI subtitleText;

    public ComputerUI ComputerUI;
    private Coroutine subtitleCoroutine;

    public List<PatientData> patientsList = new List<PatientData>();
    public List<DiseaseData> diseaseList = new List<DiseaseData>();
    public List<PatientController> patients = new List<PatientController>();
    public PatientController currentPatientController;
    public bool OnConsultation = false;

    public int currentDiseaseIndex;
    public int MaxQuestion = 1;

    public AudioSource audioSource;
    public AudioClip SFX_NextPatient;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void CallNextPatient()
    {
        StartCoroutine(CallNextPatientRoutine());
    }

    private IEnumerator CallNextPatientRoutine()
    {
        audioSource.PlayOneShot(SFX_NextPatient);

        UIManager.Instance.FadeIn(duration: 0.5f, null);
        yield return new WaitForSeconds(0.55f);

        if (currentPatientController != null)
        {
            currentPatientController.gameObject.SetActive(false);
            currentPatientController.StopAllCoroutines();
        }

        foreach (var p in patients) p.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);


        if (currentDiseaseIndex < MaxQuestion)
        {
            currentPatientController = patients[currentDiseaseIndex];
            currentPatientController.gameObject.SetActive(true);
            doctorCamera.DisableMovement();
            doctorCamera.LookAtTarget(computer);
            // patientModel.SetActive(true);
            computerInfo.SetActive(true);

            PatientData randomPatient = patientsList[Random.Range(0, patientsList.Count)];
            DiseaseData currentDisease = diseaseList[currentDiseaseIndex];
            currentPatientController.Setup(randomPatient, currentDisease);
            ComputerUI.Setup(currentPatientController.currentPatient, currentPatientController.currentDisease);
            currentDiseaseIndex++;

            yield return new WaitForSeconds(0.2f);
            UIManager.Instance.FadeOut(duration: 0.5f, () =>
            {
                doctorCamera.EnableMovement();
            });
        }
        else
        {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("Gameover");
        }

    }

    public void ShowSubtitle(string text, float duration)
    {
        if (subtitleCoroutine != null)
            StopCoroutine(subtitleCoroutine);
        subtitleCoroutine = StartCoroutine(ShowSubtitleRoutine(text, duration));
    }

    private IEnumerator ShowSubtitleRoutine(string text, float duration)
    {
        subtitleText.text = text;
        yield return new WaitForSeconds(duration);
        yield return new WaitForSeconds(1f);
        subtitleText.text = "";
    }
}
