using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource loopSource;

    [Header("Audio Clips")]
    public AudioClip running;
    public AudioClip backGround;

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

    private void Start()
    {
        musicSource.clip = backGround;
        musicSource.Play();
        loopSource.loop = true;
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void PlayLoop(AudioClip clip)
    {
        if (loopSource.clip == clip && loopSource.isPlaying) return;
        loopSource.clip = clip;
        loopSource.Play();
    }

    public void StopLoop()
    {
        if (loopSource.isPlaying) loopSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }
}
