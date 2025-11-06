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

    [Header("Button Sound")]
    public AudioClip buttonClickSound; // เสียงตอนกดปุ่ม
    private AudioSource buttonAudioSource; // แหล่งเสียงสำหรับเล่นเฉพาะเสียงปุ่ม

    private void Start()
    {
        // ✅ เพิ่ม AudioSource สำหรับเสียงปุ่ม
        buttonAudioSource = gameObject.AddComponent<AudioSource>();
        buttonAudioSource.playOnAwake = false;
        buttonAudioSource.spatialBlend = 0f; // เสียง 2D

        // ตั้งค่าเสียงเริ่มต้นที่ 50%
        SetInitialVolumes();
        
        // เริ่มต้นแสดงหน้า Main Menu และซ่อน Settings
        ShowMainMenu();
        
        // โหลดค่าตั้งต้นจาก PlayerPrefs
        LoadSettings();
    }

    // === ฟังก์ชันเล่นเสียงปุ่ม ===
    private void PlayButtonClick()
    {
        if (buttonClickSound != null && buttonAudioSource != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickSound, soundSlider != null ? soundSlider.value : 0.5f);
        }
    }

    // === ตั้งค่าเสียงเริ่มต้นที่ 50% ===
    private void SetInitialVolumes()
    {
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
        PlayButtonClick();
        SceneManager.LoadScene("PlayScenes");
    }

    public void OnSettingsButtonClicked()
    {
        PlayButtonClick();
        ShowSettings();
    }

    public void OnQuitButtonClicked()
    {
        PlayButtonClick();
        Debug.Log("Quitting game...");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // === ปุ่ม SETTINGS ===
    public void OnMusicVolumeChanged()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetMusicVolume(musicSlider.value);
        }
        SaveSettings();
    }

    public void OnSoundVolumeChanged()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetSoundVolume(soundSlider.value);
        }
        SaveSettings();
    }

    public void OnBackButtonClicked()
    {
        PlayButtonClick();
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
            PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        
        if (soundSlider != null)
            PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
        
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float soundVolume = PlayerPrefs.GetFloat("SoundVolume", 0.5f);

        if (musicSlider != null)
            musicSlider.value = musicVolume;

        if (soundSlider != null)
            soundSlider.value = soundVolume;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetMusicVolume(musicVolume);
            AudioManager.instance.SetSoundVolume(soundVolume);
        }

        Debug.Log($"โหลดการตั้งค่าเสียง: Music={musicVolume}, Sound={soundVolume}");
    }
}
