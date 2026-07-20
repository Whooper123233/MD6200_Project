using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [SerializeField] private GameObject loaderCanvas;
    [SerializeField] private Image progressBar;
    [SerializeField] private float minLoadTime = 1.5f;   
    [SerializeField] private float fillSpeed = 3f;         

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        loaderCanvas.SetActive(true);
        progressBar.fillAmount = 0f;

        float elapsed = 0f;
        float displayedProgress = 0f;

        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        while (elapsed < minLoadTime || scene.progress < 0.9f)
        {
            elapsed += Time.deltaTime;

            float targetProgress = Mathf.Clamp01(scene.progress / 0.9f);
            displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, fillSpeed * Time.deltaTime);
            progressBar.fillAmount = displayedProgress;

            yield return null;
        }

        while (displayedProgress < 1f)
        {
            displayedProgress = Mathf.MoveTowards(displayedProgress, 1f, fillSpeed * Time.deltaTime);
            progressBar.fillAmount = displayedProgress;
            yield return null;
        }

        scene.allowSceneActivation = true;

        yield return null;

        loaderCanvas.SetActive(false);
    }
}
