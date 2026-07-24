using UnityEngine;

public class HeadLookAt : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The object the head should look at.")]
    public Transform target;

    [Header("Rotation Offsets (Degrees)")]
    public Vector3 offsetRotation = Vector3.zero;

    [Header("Constraints & Limits")]
    [Tooltip("How fast the head turns towards the target.")]
    public float smoothing = 5.0f;
    [Tooltip("Maximum horizontal angle (left/right) the head can turn.")]
    public float maxYaw = 25.0f;
    [Tooltip("Maximum vertical angle (up/down) the head can look.")]
    public float maxPitch = 50.0f;

    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.localRotation;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Quaternion offset = Quaternion.Euler(offsetRotation);

        Quaternion desiredRotation = targetRotation * offset;

        Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.unscaledDeltaTime * smoothing);

        transform.rotation = ConstrainRotation(smoothedRotation);
    }

    Quaternion ConstrainRotation(Quaternion currentRot)
    {
        Quaternion parentRotation = transform.parent != null ? transform.parent.rotation : Quaternion.identity;
        Quaternion localRot = Quaternion.Inverse(parentRotation) * currentRot;

        Vector3 euler = localRot.eulerAngles;

        euler.x = NormalizeAngle(euler.x);
        euler.y = NormalizeAngle(euler.y);
        euler.z = NormalizeAngle(euler.z);

        euler.x = Mathf.Clamp(euler.x, -maxPitch, maxPitch);

        euler.y = Mathf.Clamp(euler.y, -maxYaw, maxYaw);

        euler.z = 0f;

        Quaternion constrainedLocalRot = Quaternion.Euler(euler);
        return parentRotation * constrainedLocalRot;
    }

    float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }
}