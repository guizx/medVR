using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlatformType
{
    TOTEM,
    OCULUS_QUEST,
    WINDOWS,
}

[System.Serializable]
public class PatientList
{
    public PatientData PatientData;
    public DiseaseData DiseaseData;
    public PatientController PatientController;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlatformType PlatformType;
    [SerializeField] private GameObject patientModel;
    [SerializeField] private GameObject computerInfo;
    [SerializeField] private FirstPersonCamera doctorCamera;
    [SerializeField] private Transform computer;
    [SerializeField] private TextMeshProUGUI subtitleText;

    public ComputerUI ComputerUI;
    private Coroutine subtitleCoroutine;

    public List<PatientList> PatientList = new List<PatientList>();
    public PatientController currentPatientController;
    public bool OnConsultation = false;

    public int currentDiseaseIndex;
    public int MaxQuestion = 1;

    public AudioSource audioSource;
    public AudioClip SFX_NextPatient;

    public GameObject DoctorCamera;
    public GameObject ComputerCamera;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ComputerUI.OnOptionsDisplayed += OnOpsionsDisplayed;
        ComputerUI.OnCorrectSymptomSelected += OnCorrectSymptomSelected;
        ShufflePatientList();
    }

    private void OnDestroy()
    {
        ComputerUI.OnOptionsDisplayed -= OnOpsionsDisplayed;
        ComputerUI.OnCorrectSymptomSelected -= OnCorrectSymptomSelected;

    }
    private void OnOpsionsDisplayed()
    {
        ShowOnlyComputerCamera(delay: 0.5f);
    }

    private void OnCorrectSymptomSelected()
    {
        EnableComputerCamera(false);
        EnableDoctorCamera(true);
    }

    private IEnumerator EnableComputerCameraRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        ComputerUI.ShowDisplay();
        EnableComputerCamera(true);
        EnableDoctorCamera(false);
    }

    private IEnumerator EnableDoctorCameraRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        EnableComputerCamera(false);
        EnableDoctorCamera(true);
    }

    public void EnableComputerCamera(bool state)
    {
        if (ComputerCamera != null)
            ComputerCamera.SetActive(state);
    }

    public void EnableDoctorCamera(bool state)
    {
        if (DoctorCamera != null)
            DoctorCamera.SetActive(state);
    }

    public void ShowOnlyComputerCamera(float delay)
    {
        StartCoroutine(EnableComputerCameraRoutine(delay: delay));
    }

    public void ShowOnlyDoctorCamera(float delay)
    {
        StartCoroutine(EnableDoctorCameraRoutine(delay: delay));
    }

    public void ShufflePatientList()
    {
        for (int i = PatientList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            PatientList temporal = PatientList[i];
            PatientList[i] = PatientList[randomIndex];
            PatientList[randomIndex] = temporal;
        }
    }

    public void CallNextPatient()
    {
        if (PlatformType == PlatformType.TOTEM)
        {
            StartCoroutine(CallNextPatientTotemRoutine());

        }
        else if (PlatformType == PlatformType.WINDOWS)
        {
            StartCoroutine(CallNextPatientWindowsRoutine());

        }
    }

    private IEnumerator CallNextPatientTotemRoutine()
    {
        audioSource.PlayOneShot(SFX_NextPatient);
        if (currentPatientController != null)
        {
            currentPatientController.gameObject.SetActive(false);
            currentPatientController.StopAllCoroutines();
        }

        foreach (var patient in PatientList)
            patient.PatientController.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.1f);


        if (currentDiseaseIndex < MaxQuestion)
        {
            currentPatientController = PatientList[currentDiseaseIndex].PatientController;
            currentPatientController.gameObject.SetActive(true);
            computerInfo.SetActive(true);

            PatientData randomPatient = PatientList[currentDiseaseIndex].PatientData;
            DiseaseData currentDisease = PatientList[currentDiseaseIndex].DiseaseData;
            currentPatientController.Setup(randomPatient, currentDisease);
            ComputerUI.Setup(currentPatientController.currentPatient, currentPatientController.currentDisease);
            currentDiseaseIndex++;
            ComputerUI.HideDisplay();
            ShowOnlyDoctorCamera(delay: 0f);
        }
        else
        {
            ComputerUI.ShowDisplay();
            ComputerUI.ShowScorePanel();
        }

    }

    private IEnumerator CallNextPatientWindowsRoutine()
    {
        audioSource.PlayOneShot(SFX_NextPatient);
        UIManager.Instance.FadeIn(duration: 0.5f, null);
        yield return new WaitForSeconds(0.55f);

        if (currentPatientController != null)
        {
            currentPatientController.gameObject.SetActive(false);
            currentPatientController.StopAllCoroutines();
        }

        foreach (var p in PatientList) p.PatientController.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.1f);


        if (currentDiseaseIndex < MaxQuestion)
        {
            currentPatientController = PatientList[currentDiseaseIndex].PatientController;
            currentPatientController.gameObject.SetActive(true);
            doctorCamera.DisableMovement();
            doctorCamera.LookAtTarget(computer);
            // patientModel.SetActive(true);
            computerInfo.SetActive(true);

            PatientData randomPatient = PatientList[currentDiseaseIndex].PatientData;
            DiseaseData currentDisease = PatientList[currentDiseaseIndex].DiseaseData;
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
