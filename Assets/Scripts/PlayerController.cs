using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;

    [Header("Bow Settings")]
    public GameObject arrowPrefab;          // ลูกธนู Prefab
    public GameObject circleSwordPrefab;
    public Transform shootPoint;            // จุดยิง (ปลายธนู)
    public float arrowSpeed = 20f;          // ความเร็วลูกธนู
    public float shootCooldown = 0.5f;      // หน่วงเวลายิงซ้ำ
    private float lastShootTime;

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

        // แรงโน้มถ่วง
        // if (controller.isGrounded && velocity.y < 0)
        // {
        //     velocity.y = -2f;
        // }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // ยิงลูกธนู
        // if (Input.GetButtonDown("Fire1") && Time.time - lastShootTime > shootCooldown)
        // {
        //     ShootArrow();
        //     lastShootTime = Time.time;
        // }
    }

    void ShootArrow()
    {
        if (arrowPrefab == null || shootPoint == null)
        {
            Debug.LogWarning("⚠️ Arrow prefab หรือ ShootPoint ยังไม่ถูกกำหนดใน Inspector");
            return;
        }

        // สร้างลูกธนู
        GameObject arrow = Instantiate(
    arrowPrefab,
   shootPoint.position + shootPoint.forward * 1.0f + Vector3.up * 6.0f,
    shootPoint.rotation
);
        // ถ้ามี Rigidbody ให้ลูกธนูเคลื่อนที่ด้วยแรง
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shootPoint.forward * arrowSpeed;
        }

        // ถ้ามี Animator ให้เล่นแอนิเมชันยิง
        if (animator != null)
        {
            animator.Play("IdleDemo|Shoot", 0, 0f);
        }



    }






}
