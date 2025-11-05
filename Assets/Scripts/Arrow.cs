using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float lifeTime = 5f;
    public float damage = 10.0f; // ✅ เพิ่มค่าดาเมจลูกธนู

    void Start()
    {
        Destroy(gameObject, lifeTime);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // ถ้าโดน Player → ไม่ทำอะไร
        if (collision.gameObject.CompareTag("Player"))
            return;

        // ถ้าโดน Enemy → ลดเลือดศัตรู
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // ✅ เรียกใช้ฟังก์ชันลดเลือด
                Destroy(gameObject);
            }
        }

        // ทำลายลูกธนูหลังชนอะไรก็ได้
        
    }
}
