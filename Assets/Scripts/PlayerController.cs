using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;

    [Header("Dash Settings")]
    public float dashDistance = 8f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.0f;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float lastDashTime = -999f;

    private CharacterController controller;
    private Vector3 velocity;
    private Animator animator;
    private Transform modelTransform;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        modelTransform = animator.transform;

        // ✅ ทำให้ Player เดินทะลุ Enemy ได้
        IgnoreEnemyCollisions();
    }

    void Update()
    {
        if (isDashing)
        {
            DashMovement();
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 moveInput = new Vector3(horizontal, 0f, vertical).normalized;

        bool isMoving = moveInput.magnitude > 0f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving;

        float speed = isRunning ? runSpeed : walkSpeed;

        // ✅ เคลื่อนที่ปกติ
        if (isMoving)
        {
            controller.Move(moveInput * speed * Time.deltaTime);
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // ✅ แอนิเมชันเดิน/ยืน
        if (animator != null)
        {
            if (isMoving)
                animator.Play("IdleDemo|Run");
            else
                animator.Play("IdleDemo|Idle");
        }

        // ✅ แรงโน้มถ่วง
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // ✅ Dash เมื่อกด Shift ครั้งเดียว
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            TryDash(moveInput);
        }
    }

    // 🔹 เริ่ม Dash
    void TryDash(Vector3 moveInput)
    {
        if (Time.time - lastDashTime < dashCooldown) return;

        lastDashTime = Time.time;

        // ✅ ถ้ามีทิศการกด WASD → ใช้ทิศนั้น
        // ✅ ถ้าไม่ได้กด → ใช้ทิศทางที่ model หันอยู่
        Vector3 dashDirection = moveInput.magnitude > 0 ? moveInput.normalized : modelTransform.forward;

        // หมุน model ให้หันไปทางที่ dash ทันที
        modelTransform.rotation = Quaternion.LookRotation(dashDirection);

        StartCoroutine(PerformDash(dashDirection));
    }

    IEnumerator PerformDash(Vector3 dashDir)
    {
        isDashing = true;
        dashTimer = 0f;

        if (animator != null)
            animator.Play("IdleDemo|Dash"); // ถ้ามีแอนิเมชัน dash

        while (dashTimer < dashDuration)
        {
            controller.Move(dashDir * (dashDistance / dashDuration) * Time.deltaTime);
            dashTimer += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
    }

    void DashMovement()
    {
        velocity.y = 0; // กันแรงตก
    }
    void IgnoreEnemyCollisions()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Collider enemyCol = enemy.GetComponent<Collider>();
            Collider playerCol = GetComponent<Collider>();

            if (enemyCol != null && playerCol != null)
            {
                Physics.IgnoreCollision(playerCol, enemyCol);
            }
        }

        // ถ้ามี Enemy เกิดใหม่ภายหลัง ให้รอ 1 วิ แล้วเช็คซ้ำเรื่อย ๆ
        StartCoroutine(RepeatIgnoreEnemyCollisions());
    }

    IEnumerator RepeatIgnoreEnemyCollisions()
    {
        while (true)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            Collider playerCol = GetComponent<Collider>();

            foreach (GameObject enemy in enemies)
            {
                Collider enemyCol = enemy.GetComponent<Collider>();
                if (enemyCol != null && playerCol != null)
                {
                    Physics.IgnoreCollision(playerCol, enemyCol);
                }
            }

            yield return new WaitForSeconds(1f); // เช็คซ้ำทุก 1 วินาที เผื่อมีศัตรูเกิดใหม่
        }
    }
}