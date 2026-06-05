using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioClip defaultClickSound;
    
    [Header("Music Tracks")]
    public AudioClip gameplayMusic;
    public AudioClip winMusic;
    public AudioClip finalWinMusic;
    public AudioClip loseMusic;

    [HideInInspector] public bool isSfxOn = true;
    [HideInInspector] public bool isMusicOn = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings(); 
        }
        else Destroy(gameObject);
    }
    
    public void ChangeMusic(AudioClip newTrack, bool loopTrack = true)
    {
        if (musicSource != null && newTrack != null)
        {
            if (musicSource.clip == newTrack) return; 

            musicSource.clip = newTrack;
            musicSource.loop = loopTrack;
            musicSource.Play();
        }
    }

    public void PlayClick()
    {
        // Only play if SFX is ON
        if (isSfxOn && sfxSource != null && defaultClickSound != null)
        {
            sfxSource.PlayOneShot(defaultClickSound);
        }
    }

    public void ToggleSFX()
    {
        isSfxOn = !isSfxOn;
        PlayerPrefs.SetInt("SFX_ON", isSfxOn ? 1 : 0);
        if (sfxSource != null) sfxSource.mute = !isSfxOn; 
    }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        PlayerPrefs.SetInt("MUSIC_ON", isMusicOn ? 1 : 0);
        if (musicSource != null) musicSource.mute = !isMusicOn;
    }

    private void LoadSettings()
    {
        // 1 means true, 0 means false. It defaults to 1!
        isSfxOn = PlayerPrefs.GetInt("SFX_ON", 1) == 1;
        isMusicOn = PlayerPrefs.GetInt("MUSIC_ON", 1) == 1;
        
        if (musicSource != null) musicSource.mute = !isMusicOn;
        if (sfxSource != null) sfxSource.mute = !isSfxOn;
    }
}