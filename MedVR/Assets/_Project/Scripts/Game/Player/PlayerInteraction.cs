using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactDistance = 5f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Transform crosshair;

    public bool Freeze;

    private void Start()
    {
        
            PauseManager.OnPausedChanged += OnPauseChanged;
    }

    private void OnDestroy()
    {
        PauseManager.OnPausedChanged -= OnPauseChanged;

    }

    private void OnPauseChanged(bool isPaused)
    {
        Freeze = isPaused;
    }

    private void Update()
    {
        if(Freeze)
            return;

        UpdateCrosshairState();

        if (Input.GetMouseButtonDown(0))
        {
            if (!TryClickUI())
            {
                TryClickObject();
            }
        }
    }

    private void UpdateCrosshairState()
    {
        if (crosshair == null) return;

        bool isOverInteractable = false;

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        { position = new Vector2(Screen.width / 2, Screen.height / 2) };
        List<RaycastResult> uiResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, uiResults);

        if (uiResults.Count > 0 && uiResults[0].gameObject.GetComponent<UnityEngine.UI.Button>() != null)
        {
            isOverInteractable = true;
        }
        else
        {
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
            {
                if (hit.collider.GetComponent<DiegeticButton>() != null || 
                    hit.collider.GetComponent<PatientController>() != null)
                {
                    isOverInteractable = true;
                }
            }
        }

        crosshair.localScale = Vector3.one * (isOverInteractable ? 2f : 1f);
    }

    private bool TryClickUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        { position = new Vector2(Screen.width / 2, Screen.height / 2) };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            var button = result.gameObject.GetComponent<UnityEngine.UI.Button>();
            if (button != null && button.interactable)
            {
                button.onClick.Invoke();
                return true;
            }
        }
        return false;
    }

    private void TryClickObject()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
        {
            var btn3D = hit.collider.GetComponent<DiegeticButton>();
            if (btn3D != null) btn3D.Press();

            var patient = hit.collider.GetComponent<PatientController>();
            if (patient != null && patient.enabled) patient.StartConsultation();
        }
    }
}