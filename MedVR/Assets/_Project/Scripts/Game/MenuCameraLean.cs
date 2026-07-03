using UnityEngine;

namespace Nato.GameFeel.Effect
{
    public class MenuCameraLean : MonoBehaviour
    {
       [Header("Rotation Intensity")]
    [SerializeField] private float maxRotationAngle = 2.0f;
    [SerializeField] private float rotationSpeed = 0.5f;

    [Header("Position Intensity")]
    [SerializeField] private float positionAmount = 0.05f;
    [SerializeField] private float positionSpeed = 0.3f;

    [Header("Smooth Activation")]
    [SerializeField] private float smoothSpeed = 2.0f; 

    private Quaternion originalRotation;
    private Vector3 originalPosition;
    private float noiseOffset;
    private float activationLerp = 0f; 

    void Start()
    {
        originalRotation = transform.localRotation;
        originalPosition = transform.localPosition;
        noiseOffset = Random.Range(0f, 100f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        activationLerp = Mathf.Clamp01(activationLerp + Time.deltaTime * smoothSpeed);

        float time = (Time.time * rotationSpeed) + noiseOffset;
        float tiltX = (Mathf.PerlinNoise(time, 0f) - 0.5f) * 2f;
        float tiltY = (Mathf.PerlinNoise(0f, time) - 0.5f) * 2f;
        float tiltZ = (Mathf.PerlinNoise(time, time) - 0.5f) * 2f;

        Quaternion targetRotation = Quaternion.Euler(
            originalRotation.eulerAngles.x + (tiltX * maxRotationAngle),
            originalRotation.eulerAngles.y + (tiltY * maxRotationAngle),
            originalRotation.eulerAngles.z + (tiltZ * maxRotationAngle)
        );

        transform.localRotation = Quaternion.Slerp(originalRotation, targetRotation, activationLerp);

        if (positionAmount > 0)
        {
            float posTime = (Time.time * positionSpeed) + noiseOffset;
            float posX = (Mathf.PerlinNoise(posTime, 5f) - 0.5f) * positionAmount;
            float posY = (Mathf.PerlinNoise(5f, posTime) - 0.5f) * positionAmount;

            Vector3 targetPosition = originalPosition + new Vector3(posX, posY, 0);
            
            transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, activationLerp);
        }
    }
    }
}