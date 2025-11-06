using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float lifeTime = 5f;
    public float damage = 10.0f; // ✅ เพิ่มค่าดาเมจลูกธนู
    public AudioClip arrowShootSound;


    void Start()
    {
        Destroy(gameObject, lifeTime);
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.useGravity = false;

        // 🔊 เล่นเสียงยิงลูกธนู (เสียงจะเร็วขึ้นตามการยิง)
        if (AudioManager.instance != null && arrowShootSound != null)
        {
            AudioSource src = AudioManager.instance.soundSource;
            src.pitch = Random.Range(0.9f, 1.2f); // ปรับความเร็วเสียง
            AudioManager.instance.PlaySound(arrowShootSound);
            src.pitch = 1f; // รีเซ็ต pitch หลังเล่น
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
