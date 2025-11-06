using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class GameUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject gameOverPanel; 
    public GameObject winPanel;

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    public void ShowGameOverScreen()
    {
        if (gameOverPanel == null) return;
        gameOverPanel.SetActive(true);

        if (AudioManager.instance != null)
            AudioManager.instance.MuteAllSFX(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    public void ShowWinScreen()
    {
        if (winPanel == null) return;
        winPanel.SetActive(true);

        if (AudioManager.instance != null)
            AudioManager.instance.MuteAllSFX(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }




    private IEnumerator ShowWinDelayed()
    {
        winPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 0f;
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("StartScenes");
        if (AudioManager.instance != null)
    AudioManager.instance.MuteAllSFX(false);

    }
}