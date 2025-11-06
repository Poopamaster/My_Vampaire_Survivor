using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float baseFireDelay = 0.5f;
    public float detectionRadius = 20f;
    private float fireTimer = 0f;

    [Header("Power-up Settings")]
    public int splashCount = 0;
    public int plusArrowCount = 0;

    [Header("Audio Settings")]
    public AudioClip shootSound;
    public float minPitch = 0.95f;
    public float maxPitch = 1.15f;
    private float nextShootSoundTime = 0f;
    private AudioSource localAudioSource; // ✅ สำหรับเล่นเสียงเอง

    void Awake()
    {
        // สร้าง AudioSource ถ้ายังไม่มี
        localAudioSource = gameObject.AddComponent<AudioSource>();
        localAudioSource.playOnAwake = false;
        localAudioSource.spatialBlend = 0f; // 2D sound
        localAudioSource.volume = 1f;
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0f)
        {
            Transform nearestEnemy = FindNearestEnemy();

            if (nearestEnemy != null)
            {
                Vector3 dir = (nearestEnemy.position - firePoint.position).normalized;
                firePoint.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z));

                Shoot();

                float delayMultiplier = Mathf.Max(0.2f, 1f - plusArrowCount * 0.15f);
                fireTimer = baseFireDelay * delayMultiplier;
            }
        }
    }

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

        // ✅ เล่นเสียงยิงโดยตรง ไม่ต้องผ่าน AudioManager
        if (shootSound != null && Time.time >= nextShootSoundTime)
        {
            localAudioSource.pitch = Random.Range(minPitch, maxPitch);
            localAudioSource.PlayOneShot(shootSound);
            localAudioSource.pitch = 1f;
            nextShootSoundTime = Time.time + 0.05f; // ป้องกันเสียงซ้อนรัวเกินไป
        }

        // ===== ยิงกระสุน =====
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
        if (other.CompareTag("SplashArrow"))
        {
            splashCount = Mathf.Min(splashCount + 1, 3);
            Destroy(other.gameObject);
            Debug.Log("✨ เก็บ SplashArrow → ยิงกระจายเพิ่ม!");
        }

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
