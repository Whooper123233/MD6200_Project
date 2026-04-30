using UnityEngine;

public class GrappleSwing : MonoBehaviour
{
    public LayerMask AttachableLayers;

    public float maxWebDistance = 15f;
    public float minWebDistance = 2f;
    public float swingForce = 20f;
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
    [SerializeField] public GrappleArea currentGrappleArea;  

    [SerializeField] float upwardBoost = 2f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        if (!isSwinging && webJoint != null)
        {
            Destroy(webJoint);
            ClearWebLine();
        }

        HandleLanding();
        HandleWebRelease();

        if ((currentGrappleArea != null && currentGrappleArea.canSwing) || isSwinging)
        {
            HandleWebShooting();
            HandleSwinging();
            HandleWallExit();
        }

    }
    void HandleLanding()
    {
        if (controllerScript.collsionInfo.below && !isSwinging)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void HandleWebShooting()
    {
        if (controllerScript.collsionInfo.below || isSwinging)
            return;

        if (Input.GetMouseButtonDown(0) && currentGrappleArea.canSwing)
        {
            Vector2 attachPoint = currentGrappleArea.GetAttachPoint();

            float distanceToTarget = Vector2.Distance(webOrigin.position, attachPoint);

            if (distanceToTarget < minWebDistance || distanceToTarget > maxWebDistance)
                return;

            webAttachPoint = attachPoint;

            AttachWeb(webAttachPoint);
            DrawWebLine();

            isSwinging = true;
            rb.linearDamping = 0.5f;
        }
    }
    void AttachWeb(Vector2 attachPoint)
    {
        playerMovement.enabled = false;
        controllerScript.enabled = false;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;

        storedVelocity = rb.linearVelocity;

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
            ClearWebLine();
            isSwinging = false;

            rb.linearDamping = 0f;

            playerMovement.enabled = true;
            controllerScript.enabled = true;

            playerMovement._velocity = storedVelocity;

            playerMovement._velocity.y += upwardBoost;
        }
    }

    void HandleWallExit()
    {
        if (!isSwinging) return;

        float checkDistance = 0.6f;

        RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Vector2.left, checkDistance, AttachableLayers);
        RaycastHit2D rightHit = Physics2D.Raycast(transform.position, Vector2.right, checkDistance, AttachableLayers);

        if (leftHit || rightHit)
        {
            ReleaseToController();
        }
    }
    void ReleaseToController()
    {
        storedVelocity = rb.linearVelocity;

        Destroy(webJoint);
        ClearWebLine();
        isSwinging = false;

        rb.linearDamping = 0f;

        playerMovement.enabled = true;
        controllerScript.enabled = true;

        playerMovement._velocity = storedVelocity;
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
