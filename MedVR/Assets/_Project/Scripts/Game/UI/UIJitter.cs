using UnityEngine;

public class UIJitter : MonoBehaviour
{
    [SerializeField] private float lerpSpeed = 10.0f;

    [Header("Position Settings")]
    [SerializeField] private bool jitterPosition = true;
    [SerializeField] private float speedPositionIntensity = 3.0f;
    [SerializeField] private float positionIntensity = 1.0f;

    [Header("Rotation Settings")]
    [SerializeField] private bool jitterRotation = true;
    [SerializeField] private float speedRotationIntensity = 3.0f;
    [SerializeField] private float rotationIntensity = 2.0f;

    private Vector3 initialPosition;
    private float initialRotationZ;

    private Vector3 targetPosition;
    private float targetRotationZ;

    [field: SerializeField] public bool IsActive { get; set; } = true;

    private void Start()
    {
        Reset();
    }

    public void Reset()
    {
        initialPosition = transform.localPosition;
        initialRotationZ = transform.localRotation.eulerAngles.z;

        targetPosition = initialPosition;
        targetRotationZ = initialRotationZ;
    }

    private void Update()
    {
        if (!IsActive) return;

        float timeJitterPosition = Time.unscaledTime * speedPositionIntensity;
        float timeJitterRotation = Time.unscaledTime * speedRotationIntensity;

        if (jitterPosition)
        {
            float noiseX = Mathf.Sin(timeJitterPosition * 1.2f) * positionIntensity;
            float noiseY = Mathf.Cos(timeJitterPosition * 1.1f) * positionIntensity;

            targetPosition = initialPosition + new Vector3(noiseX, noiseY, 0);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.unscaledDeltaTime * lerpSpeed);
        }

        if (jitterRotation)
        {
            float noiseRot = Mathf.Sin(timeJitterRotation * 0.9f) * rotationIntensity;

            targetRotationZ = initialRotationZ + noiseRot;

            float currentRotZ = transform.localRotation.eulerAngles.z;
            float lerpedRotZ = Mathf.LerpAngle(currentRotZ, targetRotationZ, Time.unscaledDeltaTime * lerpSpeed);

            transform.localRotation = Quaternion.Euler(0, 0, lerpedRotZ);
        }
    }
}