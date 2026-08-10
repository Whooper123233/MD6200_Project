using System.Collections;
using UnityEngine;

public class SaveButton : MonoBehaviour
{
    [SerializeField] private GameObject savedConfirmationText;
    [SerializeField] private float confirmationDuration = 1.5f;

    public void OnSavePressed()
    {
        SaveManager.Instance.Save();
        StartCoroutine(ShowConfirmation());
    }

    private IEnumerator ShowConfirmation()
    {
        savedConfirmationText.SetActive(true);
        yield return new WaitForSeconds(confirmationDuration);
        savedConfirmationText.SetActive(false);
    }
}
