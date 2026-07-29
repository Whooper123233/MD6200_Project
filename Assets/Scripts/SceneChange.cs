using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public float changeTime;
    public string sceneName;
    // Update is called once per frame
    void Update()
    {
        changeTime -= Time.deltaTime;
        if(changeTime <= 0)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
