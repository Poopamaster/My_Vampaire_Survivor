using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public Slider healthBar;
    public Image healthFill;

    // --- 1. เพิ่มบรรทัดนี้ ---
    // (ลาก GameUIManager มาใส่ในช่องนี้)
    [Header("Game Over")]
    public GameUIManager uiManager; 

    [Header("Damage Settings")]
    public float invincibilityTime = 0.5f;
    public float flashSpeed = 0.1f;

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
            Die(); // <-- เรียกฟังก์ชัน Die()
        else
            StartCoroutine(InvincibilityFlash());
    }

    public void Heal(float amount)
    {
        // ... (โค้ดส่วนนี้เหมือนเดิม) ...
        if (isDead) return;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        // ... (โค้ดส่วนนี้เหมือนเดิม) ...
        if (healthBar != null)
            healthBar.value = currentHealth;
        if (healthFill != null)
        {
            float t = currentHealth / maxHealth;
            healthFill.color = Color.Lerp(Color.red, Color.green, t);
        }
    }

    IEnumerator InvincibilityFlash()
    {
        // ... (โค้ดส่วนนี้เหมือนเดิม) ...
        isInvincible = true;
        float timer = 0f;
        while (timer < invincibilityTime)
        {
            foreach (Renderer r in renderers)
                r.enabled = !r.enabled;
            yield return new WaitForSeconds(flashSpeed);
            timer += flashSpeed;
        }
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

        // --- 2. เพิ่ม 3 บรรทัดนี้! ---
        // (นี่คือการ "เรียก" หน้า Game Over)
        if (uiManager != null)
        {
            uiManager.ShowGameOverScreen();
        }
        else
        {
            Debug.LogError("uiManager is not assigned in PlayerHealth!");
        }
    }
}