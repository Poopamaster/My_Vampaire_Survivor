using UnityEngine;
using TMPro;

public class WaveUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI timerText;

    private EnemyWaveSpawner waveSpawner;
    
    // --- 1. เพิ่ม 2 บรรทัดนี้ ---
    private GameUIManager uiManager;   // ตัวอ้างอิงถึง "สมอง" (GameUIManager)
    private bool gameHasBeenWon = false; // "ธง" กันไม่ให้เรียกซ้ำ

    void Start()
    {
        waveSpawner = FindObjectOfType<EnemyWaveSpawner>();
        
        // --- 2. ค้นหา GameUIManager เมื่อเริ่มเกม ---
        uiManager = FindObjectOfType<GameUIManager>();
    }

    void Update()
    {
        // 3. ป้องกันไม่ให้ทำงาน ถ้าเกมชนะไปแล้ว
        if (gameHasBeenWon) return; 
        
        UpdateWaveUI();
    }

    void UpdateWaveUI()
    {
        if (waveSpawner == null) return;

        // แสดง Wave ปัจจุบัน (เหมือนเดิม)
        if (waveText != null)
            waveText.text = $"WAVE: {waveSpawner.currentRound}";

        // แสดงเวลาที่เหลือ (เหมือนเดิม)
        if (timerText != null)
        {
            if (waveSpawner.IsInBreakTime())
            {
                // ... (โค้ดส่วนนี้เหมือนเดิม) ...
                float timeRemaining = waveSpawner.GetBreakTimeRemaining();
                timerText.text = $"NEXT WAVE IN: {timeRemaining:F1}s";
                timerText.color = Color.yellow;
            }
            else if (waveSpawner.isSpawning)
            {
                // ... (โค้ดส่วนนี้เหมือนเดิม) ...
                float timeRemaining = waveSpawner.GetRoundTimeRemaining();
                timerText.text = $"TIME: {timeRemaining:F1}s";
                timerText.color = Color.white;
            }
            
            // --- 4. "แก้ไข" ส่วนนี้! ---
            else if (waveSpawner.currentRound >= waveSpawner.totalRounds)
            {
                // จบเกม (ชนะแล้ว!)
                
                // ตรวจสอบว่า uiManager พร้อม และเรายังไม่ได้เรียก
                if (!gameHasBeenWon && uiManager != null)
                {
                    Debug.Log("WaveUI สั่งให้ GameUIManager โชว์หน้าจอชนะ!");
                    
                    // "สั่ง" ให้ GameUIManager โชว์หน้า "YOU WIN!"
                    uiManager.ShowWinScreen(); 
                    
                    // ตั้งธงว่า "ชนะแล้ว" (จะได้ไม่เรียกซ้ำในเฟรมถัดไป)
                    gameHasBeenWon = true; 
                    
                    // (แนะนำ) ซ่อน WaveUI นี้ไปเลย จะได้ไม่เกะกะ
                    gameObject.SetActive(false); 
                }
            }
            else
            {
                // ... (โค้ดส่วนนี้เหมือนเดิม) ...
                timerText.text = "STARTING...";
                timerText.color = Color.gray;
            }
        }
    }
}