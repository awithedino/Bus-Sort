using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance;

    public AudioSource sfxSource;
    public AudioClip defaultClickSound;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayClick()
    {
        if (sfxSource != null && defaultClickSound != null)
        {
            sfxSource.PlayOneShot(defaultClickSound);
        }
    }
}