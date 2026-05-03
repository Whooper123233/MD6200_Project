using System.Collections;
using UnityEngine;

namespace DialogueSystem
{
    public class DialogueHolder : MonoBehaviour
    {
        private void Awake()
        {
            StartCoroutine(DialogueSequence());
        }

        private IEnumerator DialogueSequence()
        {
           
            Deactivate();

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var dialogueLine = child.GetComponent<DialogueLine>();

                if (dialogueLine == null)
                {
                    Debug.LogWarning($"Missing DialogueLine on {child.name}");
                    continue;
                }

                child.gameObject.SetActive(true);

                yield return new WaitUntil(() => dialogueLine.finished);

                child.gameObject.SetActive(false); 
            }

            gameObject.SetActive(false);
        }

        private void Deactivate()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}