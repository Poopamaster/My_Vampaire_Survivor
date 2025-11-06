using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public enum EnemyType { Melee, Ranged, AOE }
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

    [Header("AOE Settings (สำหรับมังกร)")]
    public GameObject aoePrefab;          // Prefab เอฟเฟกต์ไฟ/ระเบิด
    public float aoeCastDelay = 0.8f;     // หน่วงก่อนปล่อย AOE
    public AudioClip roarSound;           // เสียงคำรามก่อนปล่อย
    public AudioClip aoeSound;            // เสียงตอนพ่นไฟ/ระเบิด

    private Transform player;
    private PlayerHealth playerHealth;
    private float attackTimer;

    private Animator animator; // ✅ เพิ่มไว้เผื่อเล่นแอนิเมชันตอนพ่นไฟ

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

        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            // เดินเข้าหาผู้เล่น
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
        else
        {
            // อยู่ในระยะโจมตี
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
            Debug.Log($"{name} โจมตีใกล้ Player!");
            animator?.Play("Attack"); // ถ้ามีแอนิเมชันโจมตี

            if (playerHealth != null)
                playerHealth.TakeDamage(damage);
        }
        else if (enemyType == EnemyType.Ranged && projectilePrefab != null)
        {
            Debug.Log($"{name} ยิง Projectile!");
            animator?.Play("Attack"); // เล่นแอนิเมชันโจมตีระยะไกล

            GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb)
            {
                Vector3 dir = (player.position - firePoint.position).normalized;
                rb.velocity = dir * projectileSpeed;
            }

            Projectile p = proj.GetComponent<Projectile>();
            if (p != null)
                p.damage = damage;
        }
        else if (enemyType == EnemyType.AOE && aoePrefab != null)
        {
            StartCoroutine(CastAOEAttack());
        }
    }

    IEnumerator CastAOEAttack()
    {
        Debug.Log($"{name} เตรียมโจมตี AOE!");

        animator?.Play("FireBreath"); // ✅ เล่นแอนิเมชันพ่นไฟถ้ามี

        if (AudioManager.instance != null && roarSound != null)
            AudioManager.instance.PlaySound(roarSound);

        yield return new WaitForSeconds(aoeCastDelay);

        // ✅ AOE จะเกิดตรงตำแหน่ง Player ตอนนั้นพอดี
        Vector3 targetPos = player.position;
        targetPos.y = 0f;

        GameObject aoe = Instantiate(aoePrefab, targetPos, Quaternion.identity);

        // ✅ ส่งค่าดาเมจให้ prefab AOEAttack.cs
        AOEAttack aoeScript = aoe.GetComponent<AOEAttack>();
        if (aoeScript != null)
        {
            aoeScript.damage = damage;
        }

        // ✅ เสียงพ่นไฟ/ระเบิด
        if (AudioManager.instance != null && aoeSound != null)
            AudioManager.instance.PlaySound(aoeSound);

        Debug.Log($"{name} ใช้ท่า AOE ใส่ผู้เล่น!");
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
        Debug.Log($"{name} ถูกฆ่าแล้ว!");

        if (WaveItemSpawner.Instance != null)
            WaveItemSpawner.Instance.TrySpawnItem(transform.position);

        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null)
        {
            if (HasAnimation(anim, "Die"))
                anim.Play("Die");
            else
                Debug.LogWarning($"{name} ไม่มี state 'Die' ใน Animator Controller!");
        }

        Destroy(gameObject, 0.5f);
    }

    bool HasAnimation(Animator animator, string stateName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == stateName)
                return true;
        }
        return false;
    }


    bool EnemyWaveSpawnerInstanceExists()
    {
        return FindObjectOfType<EnemyWaveSpawner>() != null;
    }
}
