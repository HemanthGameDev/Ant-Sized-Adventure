using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicController : MonoBehaviour
{
    public static BackgroundMusicController Instance;

    [Header("Audio Settings")]
    public AudioClip backgroundMusicClip;
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    public bool playOnStart = true;
    public bool persistAcrossScenes = true;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton pattern to avoid duplicates
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (persistAcrossScenes)
            DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        SetupAudioSource();
    }

    void SetupAudioSource()
    {
        if (backgroundMusicClip == null)
        {
            Debug.LogWarning("Background music clip not assigned.");
            return;
        }

        audioSource.clip = backgroundMusicClip;
        audioSource.loop = true;
        audioSource.volume = musicVolume;
        audioSource.playOnAwake = playOnStart;
        audioSource.spatialBlend = 0f; // 2D sound

        if (playOnStart)
            audioSource.Play();
    }

    public void SetVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        audioSource.volume = musicVolume;
    }

    public void PlayMusic()
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public void StopMusic()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }
}
