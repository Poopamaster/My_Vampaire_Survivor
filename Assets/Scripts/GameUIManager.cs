using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections; // <-- 1. ต้องเพิ่มบรรทัดนี้ เพื่อใช้ LoadScene

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

    public void ShowWinScreen()
    {
        if (winPanel == null) return;

        StopAllCoroutines();

        winPanel.SetActive(true);

        // ป้องกันการถูกปิดอัตโนมัติ
        winPanel.transform.SetAsLastSibling(); // เอามาอยู่หน้าสุดใน Canvas

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("✅ WinPanel ถูกเปิดและจะอยู่ค้างไว้จนผู้เล่นกดปุ่ม Quit");
    }



    private IEnumerator ShowWinDelayed()
    {
        winPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // รอ 0.1 วินาทีให้ UI Render ก่อน
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 0f;
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