using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource;    // สำหรับเพลงพื้นหลังเท่านั้น
    public AudioSource soundSource;    // สำหรับทุกเสียง效果 (ปุ่ม, 効果, ฯลฯ)

    [Header("Music Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip gameMusic;

    [Header("Sound Clips")]
    public AudioClip buttonClickSound;
    // สามารถเพิ่มเสียงอื่นๆ ในอนาคตได้ที่นี่
    // public AudioClip jumpSound;
    // public AudioClip attackSound;
    // public AudioClip gameOverSound;

    public static AudioManager instance;

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // ตั้งค่าเสียงเริ่มต้น
        SetInitialVolumes();
        
        // เล่นเพลงเริ่มต้นตาม scene
        PlaySceneMusic(SceneManager.GetActiveScene().name);
        
        // ฟังการเปลี่ยน scene
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // เมื่อเปลี่ยน scene ให้เปลี่ยนเพลงตาม scene
        PlaySceneMusic(scene.name);
    }

    private void PlaySceneMusic(string sceneName)
    {
        if (musicSource == null) return;

        AudioClip musicToPlay = null;

        // กำหนดเพลงตาม scene
        switch (sceneName)
        {
            case "MainMenu":
            case "MainMenuScene":
                musicToPlay = mainMenuMusic;
                break;
            case "PlayScenes":
            case "GameScene":
                musicToPlay = gameMusic;
                break;
            default:
                musicToPlay = mainMenuMusic; // default fallback
                break;
        }

        // ถ้าเพลงเปลี่ยนให้เล่นเพลงใหม่
        if (musicToPlay != null && musicSource.clip != musicToPlay)
        {
            musicSource.clip = musicToPlay;
            musicSource.Play();
        }
    }

    // === ฟังก์ชันปรับเสียง ===
    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }

    public void SetSoundVolume(float volume)
    {
        if (soundSource != null)
        {
            soundSource.volume = volume;
        }
    }

    // === ฟังก์ชันเล่นเสียง ===
    public void PlayButtonClick()
    {
        PlaySound(buttonClickSound);
    }

    public void PlaySound(AudioClip clip)
    {
        if (soundSource != null && clip != null)
        {
            soundSource.PlayOneShot(clip);
        }
    }

    private void SetInitialVolumes()
    {
        // ตั้งค่าเสียงเริ่มต้นที่ 50%
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float soundVolume = PlayerPrefs.GetFloat("SoundVolume", 0.5f);

        SetMusicVolume(musicVolume);
        SetSoundVolume(soundVolume);

        // บันทึกค่าเริ่มต้นถ้ายังไม่มี
        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", 0.5f);
        }
        if (!PlayerPrefs.HasKey("SoundVolume"))
        {
            PlayerPrefs.SetFloat("SoundVolume", 0.5f);
        }
        PlayerPrefs.Save();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}