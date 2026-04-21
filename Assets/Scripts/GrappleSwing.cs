using UnityEngine;

public class GrappleSwing : MonoBehaviour
{
    public LayerMask AttachableLayers;

    public float maxWebDistance = 15f;
    public float minWebDistance = 2f;
    public float swingForce = 1f;
    public float normalJumpForce = 7f;

    private Vector2 webAttachPoint;
    private Vector2 storedVelocity;

    public bool isSwinging = false;

    public LineRenderer webLine;
    public Transform webOrigin;
    private DistanceJoint2D webJoint;
    private Rigidbody2D rb;
    public Controller2D controllerScript;
    public PlayerMovement playerMovement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {

        if (!isSwinging)
        {
            Destroy(webJoint);
            ClearWebLine();
            isSwinging = false;
            rb.linearDamping = 0f;
        }
        HandleWebShooting();
        HandleSwinging();
        HandleWebRelease();
    }

    void HandleWebShooting()
    {
        if (controllerScript.collsionInfo.below || isSwinging)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 aimDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - webOrigin.position;
            RaycastHit2D hit = Physics2D.Raycast(webOrigin.position, aimDirection.normalized, maxWebDistance, AttachableLayers);

            if (hit.collider != null)
            {
                float distanceToTarget = Vector2.Distance(webOrigin.position, hit.point);

                if (distanceToTarget < minWebDistance)
                {
                    return;
                }

                webAttachPoint = hit.point;
                AttachWeb(webAttachPoint);
                DrawWebLine();
                isSwinging = true;
                rb.linearDamping = 0.5f;
            }
        }
    }
    void AttachWeb(Vector2 attachPoint)
    {
        storedVelocity = rb.linearVelocity;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = storedVelocity;

        webJoint = gameObject.AddComponent<DistanceJoint2D>();
        webJoint.connectedAnchor = attachPoint;
        webJoint.autoConfigureDistance = false;
        webJoint.distance = Vector2.Distance(webOrigin.position, attachPoint);
        webJoint.enableCollision = true;
    }
    void HandleSwinging()
    {
        if (webJoint != null)
        {
            Vector2 forceDirection = new Vector2(Input.GetAxis("Horizontal"), 0);
            rb.AddForce(forceDirection * swingForce);
        }
    }
    void HandleWebRelease()
    {
        if (webJoint != null && Input.GetMouseButtonUp(0))
        {
            storedVelocity = rb.linearVelocity;

            Destroy(webJoint);
            rb.bodyType = RigidbodyType2D.Kinematic;
            ClearWebLine();
            isSwinging = false;
            rb.linearDamping = 0f;

            ApplyReleaseJump();
        }
    }
    void ApplyReleaseJump()
    {
        Vector2 jumpForce = new Vector2(rb.linearVelocity.x, normalJumpForce);
        rb.AddForce(jumpForce, ForceMode2D.Impulse);
    }
    void DrawWebLine()
    {
        webLine.positionCount = 2;
        webLine.SetPosition(0, webOrigin.position);
        webLine.SetPosition(1, webAttachPoint);
    }
    void ClearWebLine()
    {
        webLine.positionCount = 0;
    }
    void LateUpdate()
    {
        if (webJoint != null)
        {
            DrawWebLine();
        }
    }

}
