using UnityEngine;

public class HeadLookAt : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The object the head should look at.")]
    public Transform target;

    [Header("Rotation Offsets (Degrees)")]
    public Vector3 offsetRotation = Vector3.zero;

    [Header("Constraints")]
    [Tooltip("How fast the head turns towards the target.")]
    public float smoothing = 5.0f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        Quaternion offset = Quaternion.Euler(offsetRotation);
        
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation * offset, Time.deltaTime * smoothing);
    }
}