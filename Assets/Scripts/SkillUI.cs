using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillUI : MonoBehaviour
{
    [Header("Skill Settings")]
    public KeyCode skillKey = KeyCode.E;
    public float skillCooldown = 20f;
    
    [Header("UI References")]
    public Image skillIcon;
    public Image cooldownOverlay;
    public TextMeshProUGUI cooldownText;
    public TextMeshProUGUI keyText;

    private bool isCooldown = false;
    private float currentCooldown = 0f;

    void Start()
    {
        if (keyText != null)
            keyText.text = skillKey.ToString();
            
        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 0f; // <-- (แก้จุดที่ 1: ตรวจสอบว่าเริ่มที่ 0)
            cooldownOverlay.gameObject.SetActive(false);
        }
        
        if (cooldownText != null)
            cooldownText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(skillKey) && !isCooldown)
        {
            UseSkill();
        }

        if (isCooldown)
        {
            currentCooldown -= Time.deltaTime;
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
        StartCooldown();
    }

    void StartCooldown()
    {
        isCooldown = true;
        currentCooldown = skillCooldown;

        if (cooldownOverlay != null)
        {
            cooldownOverlay.gameObject.SetActive(true);
            cooldownOverlay.fillAmount = 0f; // <-- (แก้จุดที่ 2: เริ่มที่ 0 (ว่างเปล่า))
        }
        
        if (cooldownText != null)
            cooldownText.gameObject.SetActive(true);

        // ทำให้ icon มืดลง (กลายเป็นสีอ่อน)
        if (skillIcon != null)
            skillIcon.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
    }

    void UpdateCooldownUI()
    {
        if (cooldownOverlay != null)
        {
            // (แก้จุดที่ 3: คำนวณแบบกลับด้าน ให้มัน "เติม" จาก 0 ไป 1)
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

        // คืนค่า icon เป็นปกติ (กลับมาเข้ม)
        if (skillIcon != null)
            skillIcon.color = Color.white;
    }

    // (ฟังก์ชันอื่นๆ เหมือนเดิม)
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