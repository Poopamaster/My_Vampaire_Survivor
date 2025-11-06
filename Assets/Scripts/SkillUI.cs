using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SkillUI : MonoBehaviour
{
    [Header("Skill Settings")]
    public KeyCode skillKey = KeyCode.E;
    public float skillCooldown = 20f;

    [Header("Skill Effect Settings")]
    public float skillDamage = 50f;
    public float slowDuration = 3f;
    public float slowMultiplier = 0.5f; // ลดความเร็วลง 50%
    public GameObject skillEffectPrefab; // เอฟเฟกต์ตอนใช้สกิล (optional)

    [Header("UI References")]
    public Image skillIcon;
    public Image cooldownOverlay;
    public TextMeshProUGUI cooldownText;
    public TextMeshProUGUI keyText;

    private bool isCooldown = false;
    private float currentCooldown = 0f;

    [Header("Audio Settings")]
    public AudioClip skillSound;       // เสียงสกิล
    public AudioSource audioSource;

    void Start()
    {
        if (keyText != null)
            keyText.text = skillKey.ToString();

        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 0f;
            cooldownOverlay.gameObject.SetActive(false);
        }

        if (cooldownText != null)
            cooldownText.gameObject.SetActive(false);

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(skillKey) && !isCooldown)
        {
            UseSkill();
        }

        if (isCooldown)
        {
            currentCooldown -= Time.unscaledDeltaTime;
            UpdateCooldownUI();

            if (currentCooldown <= 0f)
            {
                ResetCooldown();
            }
        }
    }

    void UseSkill()
    {
        Debug.Log("Skill Used!");

        if (skillSound != null && audioSource != null)
            audioSource.PlayOneShot(skillSound);

        StartCoroutine(ActivateSkillEffect());
        StartCooldown();
    }

    IEnumerator ActivateSkillEffect()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        GameObject effect = null;
        float explosionRadius = 15f; // 🔹 ระยะรัศมีของสกิล (ปรับได้)
        float delayBeforeDamage = 0.7f; // 🔹 เวลารอก่อนจะทำดาเมจ (ตามจังหวะ animation)

        // 1️⃣ สร้างเอฟเฟกต์สกิลที่ตำแหน่งผู้เล่น
        if (skillEffectPrefab != null && player != null)
        {
            effect = Instantiate(skillEffectPrefab, player.transform.position + Vector3.up * 1.0f, Quaternion.identity);
        }

        // 2️⃣ รอ animation เล่นช่วงแรก
        yield return new WaitForSeconds(delayBeforeDamage);

        // 3️⃣ หาศัตรูในรัศมี explosionRadius
        Collider[] hitColliders = Physics.OverlapSphere(player.transform.position, explosionRadius);

        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("Enemy"))
            {
                EnemyController enemyCtrl = col.GetComponent<EnemyController>();
                if (enemyCtrl != null)
                {
                    float originalSpeed = enemyCtrl.moveSpeed;

                    enemyCtrl.TakeDamage(skillDamage);

                    if (enemyCtrl.health > 0)
                        StartCoroutine(SlowEnemy(enemyCtrl, originalSpeed));
                }
            }
        }

        // 4️⃣ ทำลาย effect หลัง animation เล่นจบ
        if (effect != null)
        {
            Animator anim = effect.GetComponent<Animator>();
            if (anim != null)
            {
                float animLength = anim.GetCurrentAnimatorStateInfo(0).length;
                Destroy(effect, animLength + 0.1f);
            }
            else
            {
                Destroy(effect, 1f); // fallback
            }
        }
    }


    IEnumerator SlowEnemy(EnemyController enemy, float originalSpeed)
    {
        enemy.moveSpeed = originalSpeed * slowMultiplier;
        yield return new WaitForSeconds(slowDuration);
        if (enemy != null)
        {
            enemy.moveSpeed = originalSpeed; // คืนความเร็ว
        }
    }

    void StartCooldown()
    {
        isCooldown = true;
        currentCooldown = skillCooldown;

        if (cooldownOverlay != null)
        {
            cooldownOverlay.gameObject.SetActive(true);
            cooldownOverlay.fillAmount = 0f;
        }

        if (cooldownText != null)
            cooldownText.gameObject.SetActive(true);

        if (skillIcon != null)
            skillIcon.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
    }

    void UpdateCooldownUI()
    {
        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 1.0f - (currentCooldown / skillCooldown);
        }

        if (cooldownText != null)
        {
            cooldownText.text = Mathf.Ceil(currentCooldown).ToString();
        }
    }

    void ResetCooldown()
    {
        isCooldown = false;
        currentCooldown = 0f;

        if (cooldownOverlay != null)
            cooldownOverlay.gameObject.SetActive(false);

        if (cooldownText != null)
            cooldownText.gameObject.SetActive(false);

        if (skillIcon != null)
            skillIcon.color = Color.white;
    }

    public bool TryUseSkill()
    {
        if (!isCooldown)
        {
            UseSkill();
            return true;
        }
        return false;
    }
}
