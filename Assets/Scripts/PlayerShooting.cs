using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float fireRate = 0.3f;

    [Header("Powerups")]
    public int splashCount = 0; // เพิ่มกระสุนกระจาย (0 = ปกติ)
    public int plusArrowCount = 0; // เพิ่มจำนวนลูกธนูที่ยิงต่อครั้ง (0 = ปกติ)

    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        int totalShoots = plusArrowCount + 1; // เช่น plusArrowCount = 1 → ยิง 2 ชุด
        float verticalOffset = 0.15f;         // เว้นตำแหน่งกระสุนที่ยิงซ้อน

        for (int p = 0; p < totalShoots; p++)
        {
            // ยิงแต่ละชุด (ใช้ splashCount กระจาย)
            if (splashCount == 0)
            {
                GameObject bullet = Instantiate(
                    bulletPrefab,
                    firePoint.position + Vector3.up * (p * verticalOffset),
                    firePoint.rotation
                );
                bullet.GetComponent<Rigidbody>().velocity = firePoint.forward * bulletSpeed;
            }
            else
            {
                int totalBullets = splashCount + 1; // เช่น splashCount = 2 → ยิง 3 ลูก
                float spreadAngle = 15f;
                float startAngle = -spreadAngle * (totalBullets - 1) / 2f;

                for (int i = 0; i < totalBullets; i++)
                {
                    float angle = startAngle + i * spreadAngle;
                    Quaternion rot = firePoint.rotation * Quaternion.Euler(0, angle, 0);

                    // offset ด้านข้างให้ออกจากศูนย์กลางเล็กน้อย
                    Vector3 offset = rot * Vector3.right * (i - (totalBullets - 1) / 2f) * 0.2f;
                    Vector3 spawnPos = firePoint.position + offset + Vector3.up * (p * verticalOffset);

                    GameObject bullet = Instantiate(bulletPrefab, spawnPos, rot);
                    bullet.GetComponent<Rigidbody>().velocity = rot * Vector3.forward * bulletSpeed;
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // เก็บ Splash Arrow → เพิ่มจำนวนกระสุนกระจาย
        if (other.CompareTag("SplashArrow"))
        {
            splashCount = Mathf.Min(splashCount + 1, 3);
            Destroy(other.gameObject);
        }

        // เก็บ Plus Arrow → เพิ่มจำนวนลูกธนูที่ยิงต่อครั้ง
        if (other.CompareTag("PlusArrow"))
        {
            plusArrowCount = Mathf.Min(plusArrowCount + 1, 3);
            Destroy(other.gameObject);
        }
    }
}
