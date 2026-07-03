using System;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Sensitivity")]
    [SerializeField] private float mouseSensitivity = 2f;

    [Header("Rotation")]
    [SerializeField] private float minPitch = -60f;
    [SerializeField] private float maxPitch = 60f;
    [SerializeField] private float minYaw = -90f;
    [SerializeField] private float maxYaw = 90f;

    private float pitch = 0f;
    private float yaw = 0f;
    private bool canMove = true;

    public bool Freeze = false;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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
        if (Freeze)
            return;

        if (!canMove)
            return;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        yaw = Mathf.Clamp(yaw, minYaw, maxYaw);

        transform.localEulerAngles = new Vector3(pitch, yaw, 0f);
    }

    public void ResetRotation(Vector3 newRotation)
    {
        transform.localEulerAngles = newRotation;
        pitch = newRotation.x;
        yaw = newRotation.y;
    }

    public void LookAtTarget(Transform target)
    {
        Vector3 direction = target.position - transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Vector3 euler = targetRotation.eulerAngles;

        float targetPitch = euler.x > 180 ? euler.x - 360 : euler.x;

        float targetYaw = euler.y > 180 ? euler.y - 360 : euler.y;
        pitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);
        yaw = Mathf.Clamp(targetYaw, minYaw, maxYaw);
        transform.localEulerAngles = new Vector3(pitch, yaw, 0f);
    }

    public void DisableMovement()
    {
        canMove = false;
    }

    public void EnableMovement()
    {
        canMove = true;
    }
}