using UnityEngine;

public class FireFly : MonoBehaviour
{
    [SerializeField] FireFlyUI flyUI;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.playerData.Fireflies++;

            flyUI.UpdateUI();

            Destroy(gameObject);
        }
    }
}
