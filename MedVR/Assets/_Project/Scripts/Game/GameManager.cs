using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject patientModel;
    [SerializeField] private GameObject computerInfo;
    [SerializeField] private FirstPersonCamera doctorCamera;

    public void CallNextPatient()
    {
        if (!patientModel.activeInHierarchy)
            StartCoroutine(CallNextPatientRoutine());
    }

    private IEnumerator CallNextPatientRoutine()
    {
        UIManager.Instance.FadeIn(duration: 0.5f, null);
        yield return new WaitForSeconds(0.55f);
        doctorCamera.DisableMovement();
        doctorCamera.ResetRotation(new Vector3(19f, -40f, 0f));
        patientModel.SetActive(true);
        computerInfo.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        UIManager.Instance.FadeOut(duration: 0.5f, () =>
        {
            doctorCamera.EnableMovement();
        });
    }
}
