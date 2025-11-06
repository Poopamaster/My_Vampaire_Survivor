using UnityEngine;
using TMPro;

public class WaveUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI timerText;

    private EnemyWaveSpawner waveSpawner;
    
    private GameUIManager uiManager;
    private bool gameHasBeenWon = false;

    void Start()
    {
        waveSpawner = FindObjectOfType<EnemyWaveSpawner>();
        
        uiManager = FindObjectOfType<GameUIManager>();
    }

    void Update()
    {
        if (gameHasBeenWon) return; 
        
        UpdateWaveUI();
    }

    void UpdateWaveUI()
    {
        if (waveSpawner == null) return;

        if (waveText != null)
            waveText.text = $"WAVE: {waveSpawner.currentRound}";

        if (timerText != null)
        {
            if (waveSpawner.IsInBreakTime())
            {
                float timeRemaining = waveSpawner.GetBreakTimeRemaining();
                timerText.text = $"NEXT WAVE IN: {timeRemaining:F1}s";
                timerText.color = Color.yellow;
            }
            else if (waveSpawner.isSpawning)
            {
                float timeRemaining = waveSpawner.GetRoundTimeRemaining();
                timerText.text = $"TIME: {timeRemaining:F1}s";
                timerText.color = Color.white;
            }
            
            else if (waveSpawner.currentRound >= waveSpawner.totalRounds)
            {
                
                if (!gameHasBeenWon && uiManager != null)
                {
                    Debug.Log("WaveUI สั่งให้ GameUIManager โชว์หน้าจอชนะ!");
                    
                    uiManager.ShowWinScreen(); 
                    
                    gameHasBeenWon = true; 
                    
                }
            }
            else
            {
                timerText.text = "STARTING...";
                timerText.color = Color.gray;
            }
        }
    }
}