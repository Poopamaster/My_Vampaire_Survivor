using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float baseFireDelay = 0.5f;   // delay พื้นฐานระหว่างยิง
    private float fireTimer = 0f;

    [Header("Power-up Settings")]
    public int splashCount = 0;          // จำนวนกระสุนกระจาย (จาก SplashArrow)
    public int plusArrowCount = 0;       // จำนวน PlusArrow

    void Update()
    {
        fireTimer -= Time.deltaTime;

        if (Input.GetButton("Fire1") && fireTimer <= 0f)
        {
            Shoot();

            // ❗ คำนวณ delay จากจำนวน PlusArrow: ยิ่งมาก ยิ่งยิงเร็ว
            float delayMultiplier = Mathf.Max(0.2f, 1f - plusArrowCount * 0.15f);
            fireTimer = baseFireDelay * delayMultiplier;
        }
    }

    void Shoot()
    {
        if (!bulletPrefab || !firePoint)
        {
            Debug.LogWarning("⚠️ bulletPrefab หรือ firePoint ยังไม่ถูกเซ็ตใน Inspector");
            return;
        }

        Debug.Log($"🔫 Shooting! splash={splashCount}, plus={plusArrowCount}");

        int totalShoots = plusArrowCount + 1; // ยิงหลายชุดจาก PlusArrow
        int totalBullets = splashCount + 1;   // ยิงกระจายจาก SplashArrow

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

                // 🚫 ป้องกันชน Player เอง
                if (playerCol && bulletCol)
                    Physics.IgnoreCollision(playerCol, bulletCol);

                if (rb != null)
                {
                    rb.useGravity = false;
                    rb.velocity = rot * Vector3.forward * bulletSpeed;
                }

                Debug.DrawRay(spawnPos, rot * Vector3.forward * 3f, Color.yellow, 1.5f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 💥 เก็บ SplashArrow → เพิ่มกระสุนกระจาย
        if (other.CompareTag("SplashArrow"))
        {
            splashCount = Mathf.Min(splashCount + 1, 3); // จำกัดสูงสุด 3
            Destroy(other.gameObject);
            Debug.Log("✨ เก็บ SplashArrow → ยิงกระจายเพิ่ม!");
        }

        // 💥 เก็บ PlusArrow → ยิงหลายชุด / ยิงเร็วขึ้น
        if (other.CompareTag("PlusArrow"))
        {
            plusArrowCount = Mathf.Min(plusArrowCount + 1, 3); // จำกัดสูงสุด 3
            Destroy(other.gameObject);
            Debug.Log("⚡ เก็บ PlusArrow → ยิงเร็วขึ้น!");
        }
    }
}
