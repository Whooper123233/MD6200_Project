using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource loopSource;

    [Header("Audio Clips")]
    public AudioClip running;
    public AudioClip backGround;

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
}
