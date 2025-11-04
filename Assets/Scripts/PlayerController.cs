using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(horizontal, 0f, vertical).normalized;

        // ถ้ามีการเคลื่อนไหว
        if (move.magnitude > 0.1f)
        {
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float speed = isRunning ? runSpeed : walkSpeed;
            controller.Move(move * speed * Time.deltaTime);
        }

        // แรงโน้มถ่วง
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
