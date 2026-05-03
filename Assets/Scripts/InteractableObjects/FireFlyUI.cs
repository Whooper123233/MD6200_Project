using TMPro;
using UnityEngine;

public class FireFlyUI : MonoBehaviour
{
    public TextMeshProUGUI fireflyText;

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        int count = GameManager.Instance.playerData.Fireflies;
        fireflyText.text = "Fireflies: " + count;
    }
}
