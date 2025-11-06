using UnityEngine;

public class CircleSword : MonoBehaviour
{
    private Transform player;
    private float rotateSpeed;
    private float radius;
    private float heightOffset;
    private float baseAngle;

    [Header("Attack Settings")]
    public float damage = 50f;
    public AudioClip swordHitSound;   // เสียงฟันโดนศัตรู
    private float nextHitSoundTime = 0f;

    void Start()
    {
        if (player == null)
            player = transform.parent;
    }

    void Update()
    {
        if (player == null) return;

        // 🔄 คำนวณตำแหน่งดาบหมุนรอบตัว
        float angle = baseAngle + Time.time * rotateSpeed;
        Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;
        transform.position = player.position + offset + Vector3.up * heightOffset;

        Vector3 dir = (transform.position - player.position).normalized;

        // ✅ ป้องกัน error LookRotation viewing vector is zero
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }

    }

    public void SetOrbit(float angle, float radius, float rotateSpeed, float heightOffset)
    {
        this.radius = radius;
        this.rotateSpeed = rotateSpeed;
        this.heightOffset = heightOffset;
        this.baseAngle = angle;

        if (player == null)
            player = transform.parent;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                // 🔊 เล่นเสียงฟันโดนศัตรู (ไม่ซ้อน)
                if (AudioManager.instance != null && swordHitSound != null)
                {
                    if (Time.time >= nextHitSoundTime)
                    {
                        AudioManager.instance.PlaySound(swordHitSound);
                        nextHitSoundTime = Time.time + 0.1f;
                    }
                }
            }
        }
    }
}
