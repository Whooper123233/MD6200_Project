using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public BoxCollider2D boxCollider;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Controller2D playerController = collision.GetComponent<Controller2D>();
            if (playerController != null)
            {
                playerController.respawnPoint = transform;
            }

            GetComponent<Collider2D>().enabled = false;
        }
    }
}
