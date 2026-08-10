using Unity.VectorGraphics;
using UnityEngine;

public class YskipButton : MonoBehaviour
{
    public string sceneName;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            LevelManager.Instance.LoadScene(sceneName);

        }
    }
}
