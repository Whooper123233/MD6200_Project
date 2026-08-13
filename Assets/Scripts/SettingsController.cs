using UnityEngine;

public class SettingsController : MonoBehaviour
{
    public GameObject OptionsCanvas;
    void Start()
    {
        OptionsCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OptionsCanvas.SetActive(!OptionsCanvas.activeSelf);
        }
    }
}
