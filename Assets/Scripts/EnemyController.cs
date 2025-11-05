using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum EnemyType { Melee, Ranged }
    public EnemyType enemyType = EnemyType.Melee;

    [Header("General Settings")]
    public float moveSpeed = 3f;
    public float health = 50f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float damage = 10f;

    [Header("Ranged Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;

    private Transform player;
    private PlayerHealth playerHealth; // ✅ อ้างอิงสคริปต์เลือดของ Player
    private float attackTimer;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }

        if (enemyType == EnemyType.Ranged && firePoint == null)
        {
            GameObject fireObj = new GameObject("FirePoint");
            fireObj.transform.parent = transform;
            fireObj.transform.localPosition = Vector3.forward * 1.5f;
            firePoint = fireObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            // เดินเข้าไปหาผู้เล่น
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
        else
        {
            // โจมตี
            if (attackTimer <= 0f)
            {
                Attack();
                attackTimer = attackCooldown;
            }
        }

        attackTimer -= Time.deltaTime;
    }

    void Attack()
    {
        if (enemyType == EnemyType.Melee)
        {
            Debug.Log($"{name} attacks player (Melee)!");

            // ✅ ถ้าอยู่ในระยะโจมตี ให้ลดเลือดผู้เล่น
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
        else if (enemyType == EnemyType.Ranged && projectilePrefab != null)
        {
            Debug.Log($"{name} attacks player (Ranged)!");
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb)
            {
                Vector3 dir = (player.position - firePoint.position).normalized;
                rb.velocity = dir * projectileSpeed;
            }

            // ✅ ตั้งค่าความเสียหายของ projectile ด้วย
            Projectile p = proj.GetComponent<Projectile>();
            if (p != null)
            {
                p.damage = damage;
            }
        }
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{name} has died!");
        Destroy(gameObject);
    }
}
