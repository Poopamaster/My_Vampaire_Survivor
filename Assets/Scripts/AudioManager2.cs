using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager2 : MonoBehaviour
{
    public static AudioManager2 instance;

    [Header("🎵 Background Music")]
    public AudioClip mainMenuBGM;    // เพลงหน้าเมนูหลัก
    public AudioClip gameplayBGM;    // เพลงหน้าเกม
    public AudioSource bgmSource;    // ตัวเล่นเพลงหลัก

    [Header("🔊 Volume Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    private void Awake()
    {
        // ✅ ทำให้ AudioManager2 คงอยู่ข้าม Scene
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // ถ้ายังไม่มี AudioSource -> เพิ่มอัตโนมัติ
            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
                bgmSource.playOnAwake = false;
                bgmSource.loop = true;
                bgmSource.spatialBlend = 0f;
            }

            // โหลดค่าระดับเสียงจาก PlayerPrefs
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            bgmSource.volume = musicVolume;

            // ✅ ผูก event เมื่อเปลี่ยน Scene
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // เล่นเพลงตาม Scene ปัจจุบัน
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        // ป้องกัน memory leak
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ✅ เมื่อเปลี่ยน Scene จะเรียกฟังก์ชันนี้โดยอัตโนมัติ
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;

        if (sceneName == "StartScenes")
        {
            PlayMainMenuMusic();
        }
        else if (sceneName == "PlayScenes")
        {
            PlayGameplayMusic();
        }
    }

    // === 🎵 ฟังก์ชันเล่นเพลงต่าง ๆ ===
    public void PlayMainMenuMusic()
    {
        PlayMusic(mainMenuBGM);
    }

    public void PlayGameplayMusic()
    {
        PlayMusic(gameplayBGM);
    }

    public void StopMusic()
    {
        if (bgmSource.isPlaying)
            bgmSource.Stop();
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        // ป้องกันการเล่นซ้ำเพลงเดิม
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.volume = musicVolume;
        bgmSource.Play();
    }

    // === 🔊 ปรับระดับเสียง (เรียกจาก GameManager) ===
    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        bgmSource.volume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }
}
