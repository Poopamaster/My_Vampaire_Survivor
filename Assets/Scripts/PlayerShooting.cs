using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float baseFireDelay = 0.5f;
    public float detectionRadius = 20f; // ระยะตรวจหาศัตรู
    private float fireTimer = 0f;

    [Header("Power-up Settings")]
    public int splashCount = 0;     // จำนวนกระสุนกระจาย
    public int plusArrowCount = 0;  // จำนวน PlusArrow
    [Header("Audio Settings")]
    public AudioClip shootSound;          // เสียงยิงลูกธนู
    public float minPitch = 0.95f;        // ค่าพิทช์ต่ำสุด
    public float maxPitch = 1.15f;        // ค่าพิทช์สูงสุด
    private float nextShootSoundTime = 0f;


    void Update()
    {
        fireTimer -= Time.deltaTime;

        // ✅ ยิงอัตโนมัติเมื่อมีเป้าหมายใกล้ที่สุด
        if (fireTimer <= 0f)
        {
            Transform nearestEnemy = FindNearestEnemy();

            if (nearestEnemy != null)
            {
                // หมุน firePoint ไปทางศัตรู
                Vector3 dir = (nearestEnemy.position - firePoint.position).normalized;
                firePoint.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z));

                // ยิงเลย
                Shoot();

                // delay ตามจำนวน plusArrow
                float delayMultiplier = Mathf.Max(0.2f, 1f - plusArrowCount * 0.15f);
                fireTimer = baseFireDelay * delayMultiplier;
            }
        }
    }

    /// <summary>
    /// 🔍 หาศัตรูที่อยู่ใกล้ Player ที่สุดในระยะที่กำหนด
    /// </summary>
    Transform FindNearestEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        Transform nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("Enemy"))
            {
                float dist = Vector3.Distance(transform.position, col.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearest = col.transform;
                }
            }
        }

        return nearest;
    }

    void Shoot()
    {
        if (!bulletPrefab || !firePoint)
        {
            Debug.LogWarning("⚠️ bulletPrefab หรือ firePoint ยังไม่ถูกเซ็ตใน Inspector");
            return;
        }

        // 🔊 เล่นเสียงยิงธนู / กระสุน
        if (AudioManager.instance != null && shootSound != null)
        {
            // ป้องกันเสียงซ้อนถ้ายิงรัวมาก
            if (Time.time >= nextShootSoundTime)
            {
                var src = AudioManager.instance.soundSource;
                src.pitch = Random.Range(minPitch, maxPitch);
                AudioManager.instance.PlaySound(shootSound);
                src.pitch = 1f;
                nextShootSoundTime = Time.time + 0.05f; // ดีเลย์สั้นๆ ป้องกันซ้อน
            }
        }

        // จากตรงนี้ลงไปคือโค้ดยิงเดิมของคุณ ↓
        int totalShoots = plusArrowCount + 1;
        int totalBullets = splashCount + 1;
        float spreadAngle = 10f;
        float startAngle = -spreadAngle * (totalBullets - 1) / 2f;
        float distanceFromPlayer = 2.0f;
        float verticalOffsetPerSet = 0.4f;
        Collider playerCol = GetComponent<Collider>();

        for (int p = 0; p < totalShoots; p++)
        {
            for (int i = 0; i < totalBullets; i++)
            {
                float angle = startAngle + i * spreadAngle;
                Quaternion rot = firePoint.rotation * Quaternion.AngleAxis(angle, Vector3.up);

                Vector3 spawnPos = firePoint.position +
                                   rot * Vector3.forward * distanceFromPlayer +
                                   Vector3.up * (verticalOffsetPerSet * p + 0.5f);

                GameObject bullet = Instantiate(bulletPrefab, spawnPos, rot);
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                Collider bulletCol = bullet.GetComponent<Collider>();

                if (playerCol && bulletCol)
                    Physics.IgnoreCollision(playerCol, bulletCol);

                if (rb != null)
                {
                    rb.useGravity = false;
                    rb.velocity = rot * Vector3.forward * bulletSpeed;
                }
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // 💥 เก็บ SplashArrow → เพิ่มกระสุนกระจาย
        if (other.CompareTag("SplashArrow"))
        {
            splashCount = Mathf.Min(splashCount + 1, 3);
            Destroy(other.gameObject);
            Debug.Log("✨ เก็บ SplashArrow → ยิงกระจายเพิ่ม!");
        }

        // 💥 เก็บ PlusArrow → ยิงหลายชุด / ยิงเร็วขึ้น
        if (other.CompareTag("PlusArrow"))
        {
            plusArrowCount = Mathf.Min(plusArrowCount + 1, 3);
            Destroy(other.gameObject);
            Debug.Log("⚡ เก็บ PlusArrow → ยิงเร็วขึ้น!");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
