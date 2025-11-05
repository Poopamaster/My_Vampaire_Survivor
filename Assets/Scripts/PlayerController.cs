using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;
    private Animator animator;
    private Transform modelTransform;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        modelTransform = animator.transform;
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(horizontal, 0f, vertical).normalized;
        bool isMoving = move.magnitude > 0f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving;

        float speed = isRunning ? runSpeed : walkSpeed;

        // เคลื่อนที่
        if (isMoving)
        {
            controller.Move(move * speed * Time.deltaTime);

            // หมุนตามทิศทาง
            Quaternion targetRotation = Quaternion.LookRotation(move);
            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // เปลี่ยนแอนิเมชัน
        if (animator != null)
        {
            if (isMoving)
                animator.Play("IdleDemo|Run");
            else
                animator.Play("IdleDemo|Idle");
        }

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // กดไว้กับพื้น
        }
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
