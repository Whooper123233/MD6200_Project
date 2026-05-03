using UnityEngine;

public class NPC_interaction : MonoBehaviour
{
    [SerializeField] private GameObject dialogue;

    public void ActivateDialouge()
    {
        dialogue.SetActive(true);
    }

    public bool DialogueActive()
    {
        return dialogue.activeInHierarchy;
    }
}
