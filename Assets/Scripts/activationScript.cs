using UnityEngine;
using UnityEngine.Playables;

public class activationScript : MonoBehaviour
{
    [SerializeField] private PlayableDirector playableDirector;
    public string sceneName;
    private bool isActivated;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isActivated)
        {
            isActivated = true;
            playableDirector.Play();
            playableDirector.stopped += OnCutsceneFinished;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    private void OnCutsceneFinished(PlayableDirector director)
    {
        playableDirector.stopped -= OnCutsceneFinished;
        SaveManager.Instance.SetIntroCutsceneSeen();
        LevelManager.Instance.LoadScene(sceneName);
    }
}