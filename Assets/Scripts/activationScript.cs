using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class activationScript : MonoBehaviour
{
    [SerializeField] private PlayableDirector playableDirector;
    public float changeTime;
    public string sceneName;

    private bool isActivated;
    private float timer;

    void Update()
    {
        if (!isActivated) return; 

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isActivated)
        {
            playableDirector.Play();
            GetComponent<BoxCollider2D>().enabled = false;

            isActivated = true;
            timer = changeTime; 
        }
    }
}
