using UnityEngine;

public class GrappleArea : MonoBehaviour
{
    public bool canSwing = false;
    private SpriteRenderer sr;

    [SerializeField] private Color flashColor = Color.yellow;
    [SerializeField] private float flashSpeed = 3f;

    public Transform attachPoint;
    private Color originalColor;
    private void Start()
    {
        sr = GetComponentInParent<SpriteRenderer>();

        if (sr != null)
            originalColor = sr.color;
    }
    private void Update()
    {
        if (sr == null) return;

        if (canSwing)
        {
            sr.color = Color.Lerp(originalColor, flashColor, Mathf.PingPong(Time.time * flashSpeed, 1));
        }
        else
        {
            sr.color = originalColor;
        }
    }
    public Vector2 GetAttachPoint()
    {
        return attachPoint != null ? attachPoint.position : transform.position;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var swing = collision.GetComponent<GrappleSwing>();
            if (swing != null)
            {
                swing.currentGrappleArea = this;
                canSwing = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var swing = collision.GetComponent<GrappleSwing>();
        if (swing != null && swing.currentGrappleArea == this)
        {
            swing.currentGrappleArea = null;
        }

        canSwing = false;
    }
}
