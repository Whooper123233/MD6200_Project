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
    [SerializeField] public GrappleArea currentGrappleArea;   //NEED THE GRAPPLE AREAD TO BE ON THE GRAPPLE SCRIPT SOME HOW CUZ IT ONLY SEE ONE AREA NOT MULTIPLE !!!

    [SerializeField] float launchForce = 20f;
    [SerializeField] float upwardBoost = 2f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (!isSwinging)
        {
            Destroy(webJoint);
            ClearWebLine();
            rb.linearDamping = 0f;
        }

        HandleLanding();
        HandleWebRelease();

        if ((currentGrappleArea != null && currentGrappleArea.canSwing) || isSwinging)
        {
            HandleWebShooting();
            HandleSwinging();
            HandleCatapult();
        }

    }
    void HandleLanding()
    {
        //NEED TO HANDLE LANDING ON A WALL AS WELL AS THE GFROUND!!!!
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
    void HandleCatapult()
    {
        if (!currentGrappleArea.canSwing)
            return;

        if (Input.GetMouseButtonDown(1) && currentGrappleArea.canSwing)
        {
            Vector2 aimDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - webOrigin.position;

            RaycastHit2D hit = Physics2D.Raycast(webOrigin.position, aimDirection.normalized, maxWebDistance, AttachableLayers);

            if (hit.collider != null)
            {
                LaunchPlayer(hit.point);
            }
        }
    }
    void LaunchPlayer(Vector2 targetPoint)
    {
      
        if (isSwinging)
        {
            Destroy(webJoint);
            ClearWebLine();
            isSwinging = false;
        }

        Vector2 direction = (targetPoint - (Vector2)transform.position).normalized;

        direction.y += upwardBoost;
        direction.Normalize();

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * launchForce, ForceMode2D.Impulse);
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

            ApplyReleaseJump();
            rb.bodyType = RigidbodyType2D.Kinematic;
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
