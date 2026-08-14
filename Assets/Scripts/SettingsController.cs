using UnityEngine;

public class SettingsController : MonoBehaviour
{
    public GameObject OptionsCanvas;
    void Start()
    {
        OptionsCanvas.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (OptionsCanvas.activeSelf)
                CloseOptions();
            else
                OpenOptions();
        }
    }

    public void OpenOptions()
    {
        OptionsCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseOptions() 
    {
        OptionsCanvas.SetActive(false);
        Time.timeScale = 1f;
    }
}
