using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    [Header("Settings Sliders")]
    public Slider musicSlider;
    public Slider soundSlider;

    private void Start()
    {
        // ตั้งค่าเสียงเริ่มต้นที่ 50%
        SetInitialVolumes();
        
        // เริ่มต้นแสดงหน้า Main Menu และซ่อน Settings
        ShowMainMenu();
        
        // โหลดค่าตั้งต้นจาก PlayerPrefs
        LoadSettings();
    }

    // ตั้งค่าเสียงเริ่มต้นที่ 50%
    private void SetInitialVolumes()
    {
        // บังคับตั้งค่าเริ่มต้น 50% ถ้ายังไม่เคยบันทึก
        if (!PlayerPrefs.HasKey("MusicVolume") || !PlayerPrefs.HasKey("SoundVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", 0.5f);
            PlayerPrefs.SetFloat("SoundVolume", 0.5f);
            PlayerPrefs.Save();
        }
    }

    // === ปุ่ม MAIN MENU ===
    public void OnPlayButtonClicked()
    {
        // เล่นเสียง effect เมื่อกดปุ่ม (อยู่ในหมวด Sound)
        AudioManager.instance.PlayButtonClick();
        
        // โหลดหน้า PlayScenes
        SceneManager.LoadScene("PlayScenes");
    }

    public void OnSettingsButtonClicked()
    {
        // เล่นเสียง effect เมื่อกดปุ่ม (อยู่ในหมวด Sound)
        AudioManager.instance.PlayButtonClick();
        
        // แสดงหน้า Settings
        ShowSettings();
    }

    public void OnQuitButtonClicked()
    {
        // เล่นเสียง effect เมื่อกดปุ่ม (อยู่ในหมวด Sound)
        AudioManager.instance.PlayButtonClick();
        
        // ออกจากเกม
        Debug.Log("Quitting game...");
        Application.Quit();
        
        // สำหรับการทดสอบใน Editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // === ปุ่ม SETTINGS ===
    public void OnMusicVolumeChanged()
    {
        // ปรับเสียง background music เท่านั้น
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetMusicVolume(musicSlider.value);
        }
        SaveSettings();
    }

    public void OnSoundVolumeChanged()
    {
        // ปรับเสียง effect ทุกชนิด (ปุ่ม, 効果เกม, ฯลฯ)
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetSoundVolume(soundSlider.value);
        }
        SaveSettings();
    }

    public void OnBackButtonClicked()
    {
        // เล่นเสียง effect เมื่อกดปุ่ม (อยู่ในหมวด Sound)
        AudioManager.instance.PlayButtonClick();
        
        // กลับไปหน้า Main Menu
        ShowMainMenu();
    }

    // === ฟังก์ชันจัดการการแสดงผล ===
    private void ShowMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    private void ShowSettings()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    // === บันทึกและโหลดการตั้งค่า ===
    private void SaveSettings()
    {
        if (musicSlider != null)
        {
            PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        }
        
        if (soundSlider != null)
        {
            PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
        }
        
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        // โหลดค่าจาก PlayerPrefs หรือใช้ค่าตั้งต้น 0.5
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float soundVolume = PlayerPrefs.GetFloat("SoundVolume", 0.5f);

        // ตั้งค่าสไลเดอร์
        if (musicSlider != null)
        {
            musicSlider.value = musicVolume;
        }

        if (soundSlider != null)
        {
            soundSlider.value = soundVolume;
        }

        // ตั้งค่าเสียงใน AudioManager
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetMusicVolume(musicVolume);
            AudioManager.instance.SetSoundVolume(soundVolume);
        }

        Debug.Log($"โหลดการตั้งค่าเสียง: Music={musicVolume}, Sound={soundVolume}");
    }
}