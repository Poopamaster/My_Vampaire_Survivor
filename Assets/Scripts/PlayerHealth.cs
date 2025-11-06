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

    [Header("Game Over")]
    public GameUIManager uiManager;

    [Header("Damage Settings")]
    public float invincibilityTime = 0.5f;
    public float flashSpeed = 0.1f;

    private bool isInvincible = false;
    public bool isDead = false;
    private Renderer[] renderers;
    [Header("Audio")]
    public AudioClip hurtSound;
     private AudioSource localAudioSource;

    void Start()
    {
        currentHealth = maxHealth;
         localAudioSource = gameObject.AddComponent<AudioSource>();
        localAudioSource.playOnAwake = false;
        localAudioSource.spatialBlend = 0f; // เล่นเป็น 2D sound
        localAudioSource.volume = 1f;

        if (healthBar == null)
        {
            healthBar = GameObject.Find("HealthBar")?.GetComponent<Slider>();
            Debug.LogWarning($"[AutoBind] healthBar not assigned — auto found: {healthBar}");
        }

        if (healthFill == null && healthBar != null)
        {
            healthFill = healthBar.fillRect.GetComponent<Image>();
        }

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
            Die();
        else
            StartCoroutine(InvincibilityFlash());

        // ✅ เล่นเสียงเจ็บโดยตรง (ไม่ผ่าน AudioManager)
        if (hurtSound != null)
        {
            localAudioSource.PlayOneShot(hurtSound);
        }
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
        CircleSwordManager circle = FindObjectOfType<CircleSwordManager>();
    if (circle != null)
        circle.StopAllSounds();

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

    void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Heart"))
    {
        Heal(50f);
        Destroy(other.gameObject);
        Debug.Log(" เก็บหัวใจ! ฟื้นฟูเลือด 50 หน่วย");
    }
}



}