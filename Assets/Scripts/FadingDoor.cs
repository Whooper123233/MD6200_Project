using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FadingDoor : MonoBehaviour
{
    [SerializeField] private int fireFliesNeeded = 5;
    [SerializeField] private float fadeDuration = 1.5f;

    private bool hasOpened = false;
    private Tilemap tilemap;
    private Collider2D doorCollider;

    private void Start()
    {
        tilemap = GetComponent<Tilemap>();
        doorCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (hasOpened) return;

        if (GameManager.Instance != null && GameManager.Instance.playerData.Fireflies >= fireFliesNeeded)
        {
            hasOpened = true;

            if (doorCollider != null)
                doorCollider.enabled = false;

            StartCoroutine(FadeOut());
        }
    }
    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color startColor = tilemap.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            Color color = startColor;
            color.a = alpha;
            tilemap.color = color;

            yield return null;
        }

        Destroy(gameObject);
    }

}
