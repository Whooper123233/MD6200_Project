using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public void QuitGame()
    {

        UnityEditor.EditorApplication.isPlaying = false; 

    }
}
