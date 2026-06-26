using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    private float currentSpeed;
    public Vector3 Movement;
    public Vector3 LoockPosition;
    public CharacterController CharacterController;
    public Animator Animator;
    public float speed = 5f;
    public float speedSmooth = 0.3f;

    public Camera playerCamera;

    public LayerMask aimingLayer;

    public float OffsetY;
    public Transform graphic;
    public float gravity = -9.81f;
    private Vector3 velocity;
    private void Update()
    {

        if (CharacterController.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(inputH, 0f, inputV).normalized;

        CharacterController.Move(inputDir * speed * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        CharacterController.Move(velocity * Time.deltaTime);
        if (inputDir.magnitude > 1)
            inputDir.Normalize();

        RotateTowardsMouse();
        graphic.transform.localEulerAngles = new Vector3(0, OffsetY, 0f);

        Vector3 localMove = transform.InverseTransformDirection(inputDir);

        Animator.SetFloat("Horizontal", Mathf.Lerp(Animator.GetFloat("Horizontal"), localMove.x, speedSmooth));
        Animator.SetFloat("Vertical", Mathf.Lerp(Animator.GetFloat("Vertical"), localMove.z, speedSmooth));
    }

    private void RotateTowardsMouse()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, aimingLayer))
        {
            LoockPosition = hit.point;
        }

        Vector3 lookDirection = LoockPosition - transform.position;
        lookDirection.y = 0f;


        transform.LookAt(transform.position + lookDirection, Vector3.up);
    }
}
