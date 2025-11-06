using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // <-- 1. ต้องเพิ่มบรรทัดนี้ เพื่อใช้ LoadScene

/// <summary>
/// สคริปต์นี้จะคุม UI ทั้งหมดในฉากเกม
/// (เช่น หน้า Game Over, หน้า Win, หน้า Pause)
/// </summary>
public class GameUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject gameOverPanel; // <-- 2. ลาก GameOverPanel มาใส่
    public GameObject winPanel;      // <-- 3. ลาก WinPanel มาใส่

    void Start()
    {
        // 4. ซ่อน Panel ทั้งหมดตอนเริ่มเกม (กันลืม)
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        if (winPanel != null)
            winPanel.SetActive(false);
    }

    // --- ฟังก์ชันนี้จะถูกเรียกจาก PlayerHealth.cs ---
    public void ShowGameOverScreen()
    {
        if (gameOverPanel == null) return;

        gameOverPanel.SetActive(true); // 5. เปิดหน้า Game Over
        
        // 6. หยุดเกม (หยุดทุกอย่างในฉาก)
        Time.timeScale = 0f; 

        // 7. โชว์เมาส์ และปลดล็อกเมาส์ (เพื่อให้คลิกปุ่ม Quit ได้)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // --- ฟังก์ชันนี้คุณจะเรียกจาก "สคริปต์อื่น" เมื่อชนะ ---
    public void ShowWinScreen()
    {
        if (winPanel == null) return;

        winPanel.SetActive(true); // เปิดหน้า Win
        Time.timeScale = 0f; // หยุดเกม
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // --- ฟังก์ชันนี้สำหรับ "ปุ่ม Quit" ---
    public void QuitToMainMenu()
    {
        // 8. (สำคัญมาก!) ต้องปรับเวลาให้กลับเป็นปกติก่อนโหลดฉากใหม่
        Time.timeScale = 1f; 
        
        // 9. กลับไปหน้าเมนู (ตามชื่อที่คุณบอก)
        SceneManager.LoadScene("StartScenes"); 
    }
}