using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public Slider healthBar;     // แถบพลังชีวิตใน Canvas
    public Image healthFill;     // ส่วนที่เป็นสี (fill) ของแถบเลือด

    [Header("Damage Settings")]
    public float invincibilityTime = 0.5f; // เวลาอมตะหลังโดนตี
    public float flashSpeed = 0.1f;        // ความเร็วการกระพริบ

    private bool isInvincible = false;
    private bool isDead = false;
    private Renderer[] renderers;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        // เก็บ Renderer ทั้งหมดของ Player ไว้ใช้กระพริบ
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead || isInvincible) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Player took {amount} damage. HP: {currentHealth}/{maxHealth}");

        UpdateHealthUI();

        if (currentHealth <= 0)
            Die();
        else
            StartCoroutine(InvincibilityFlash());
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthBar != null)
            healthBar.value = currentHealth;

        if (healthFill != null)
        {
            // ปรับสีตามเปอร์เซ็นต์เลือด
            float t = currentHealth / maxHealth;
            healthFill.color = Color.Lerp(Color.red, Color.green, t);
        }
    }

    IEnumerator InvincibilityFlash()
    {
        isInvincible = true;
        float timer = 0f;

        while (timer < invincibilityTime)
        {
            // กระพริบตัว (เปิด/ปิด Renderer)
            foreach (Renderer r in renderers)
                r.enabled = !r.enabled;

            yield return new WaitForSeconds(flashSpeed);
            timer += flashSpeed;
        }

        // เปิด Renderer กลับให้ครบ
        foreach (Renderer r in renderers)
            r.enabled = true;

        isInvincible = false;
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Player died!");

        // ปิดการควบคุม
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
            controller.enabled = false;

        // TODO: ใส่แอนิเมชันตายหรือรีเซ็ตฉาก
    }
}
