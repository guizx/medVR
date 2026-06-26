using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera targetCamera;
    public bool lockY = true;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (targetCamera == null) return;

        Vector3 lookDir = transform.position - targetCamera.transform.position;

        if (lockY)
            lookDir.y = 0f;

        if (lookDir.sqrMagnitude < 0.001f) return;

        transform.rotation = Quaternion.LookRotation(lookDir);
    }
}
