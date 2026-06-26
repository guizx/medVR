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
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
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

    public void DisableMovement()
    {
        canMove = false;
    }

    public void EnableMovement()
    {
        canMove = true;
    }
}