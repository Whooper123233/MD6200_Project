using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
        }
        Load();
    }

    public void ChangeVolume()
    {
        AudioManager.Instance.SetMusicVolume(volumeSlider.value);
        Save();
    }

    private void Load()
    {
        float saved = PlayerPrefs.GetFloat("musicVolume");
        volumeSlider.value = saved;
        AudioManager.Instance.SetMusicVolume(saved);
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
    }
}
