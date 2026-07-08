using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Assertions.Must;
using Unity.Cinemachine;

[RequireComponent(typeof(Controller2D),typeof(LineRenderer))]
public class PlayerMovement : MonoBehaviour
{
    public MovementSettings ms;
    private NPC_interaction npc_Interaction;
    private int facingDir = 1;

    [Header("Coyote Time & Input Buffer")]
    public float coyoteTime = .2f;
    private float _coyoteTimer;
    public float inputBuffer = .2f;
    private float _inputTimer;

    [Header("Interaction")]
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private LayerMask npcMask;

    [Header("Movement stuff dont need to touch")]
    public bool _isDashing;
    public float _dashTimeLeft;
    public float _dashCD;
    public int _dashDir;
    public bool _hasDashed;
    public Vector3 _velocity;
    float targetVelocityX;
    float velocityXSmoothing;
    float maxJumpVelocity;
    public float minJumpVelocity;
    public bool _justWallJumped = false;
    public bool _isWallSliding = false;

    [Header("Grapple")]
    public Controller2D controller;
    public bool isSwinging = false;
    public float overlapRadius = 5f;
    [SerializeField] public LayerMask grappleMask;
    [SerializeField] public LayerMask wallMask;
    LineRenderer lineRenderer;
    Vector2 totalVelocity;
    private Vector2 grapplePoint;
    public GrappleArea currentGrappleArea;
    private float ropeLength;


    [Header("Camera")]
    [SerializeField] private CinemachineCamera cinemachine;
    private CinemachinePositionComposer positionComposer;

    [SerializeField] private PlayerMovementStates playerMovementStates;

    void Start()
    {     
        controller = GetComponent<Controller2D>();
        positionComposer = cinemachine.GetComponent<CinemachinePositionComposer>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;  
        lineRenderer.enabled = false;

        ms.gravity = -(2 * ms.maxJumpHeight) / Mathf.Pow(ms.timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(ms.gravity) * ms.timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(ms.gravity) * ms.minJumpHeight);
    }

    void Update()
    {
        if (InDialogue()) return;

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        int wallDirX = (controller.collsionInfo.left) ? -1 : 1;

        UpdateFacing(input);
        transform.localScale = new Vector3(facingDir, 1f, 1f);
        UpdateCameraDamping();

        if (Input.GetMouseButtonDown(0) && currentGrappleArea != null)
        {
            isSwinging = true;
            HandleGrapple();
        }
        if (isSwinging)
        {
            totalVelocity.y += ms.gravity * Time.deltaTime;

            if (input.x != 0f)
            {
                Vector2 toGrappleInput = (Vector2)transform.position - grapplePoint;
                float distInput = toGrappleInput.magnitude;
                if (distInput > 0.0001f)
                {
                    Vector2 radialDirInput = toGrappleInput / distInput;
                    Vector2 tangent = new Vector2(-radialDirInput.y, radialDirInput.x);

                    if (Vector2.Dot(tangent, Vector2.right) < 0f)
                        tangent = -tangent;

                    totalVelocity += tangent * input.x * ms.swingAcceleration * Time.deltaTime;

                    if (totalVelocity.magnitude > ms.maxSwingSpeed)
                        totalVelocity = totalVelocity.normalized * ms.maxSwingSpeed;
                }
            }

        
            Vector2 toGrapple = (Vector2)transform.position - grapplePoint;
            float distance = toGrapple.magnitude;
            if (distance > 0.0001f)
            {
                Vector2 dir = toGrapple / distance;
                float radialSpeed = Vector2.Dot(totalVelocity, dir);
                totalVelocity -= dir * radialSpeed;

                float drift = distance - ropeLength;
                if (Mathf.Abs(drift) > 0.001f)
                {
                    Vector2 correction = -dir * drift * 10f * Time.deltaTime; 
                    controller.Move(correction);
                }
            }

            controller.Move(totalVelocity * Time.deltaTime);

            if (controller.collsionInfo.above && totalVelocity.y > 0) totalVelocity.y = 0;
            if (controller.collsionInfo.below && totalVelocity.y < 0) totalVelocity.y = 0;
            if (controller.collsionInfo.left && totalVelocity.x < 0) totalVelocity.x = 0;
            if (controller.collsionInfo.right && totalVelocity.x > 0) totalVelocity.x = 0;

            UpdateGrappleLine();

            if (Input.GetMouseButtonUp(0) || controller.collsionInfo.below)
            {
                isSwinging = false;
                lineRenderer.enabled = false;
                _velocity = totalVelocity;
                velocityXSmoothing = totalVelocity.x;
            }

            return;
        }
        HandleDash(input);
        HorizontalMovement(input, wallDirX);
        WallSliding(input, wallDirX);
        Jumping();

        Gravity();
        controller.Move(_velocity * Time.deltaTime);
        StopBouncing();
        CheckInteraction();
    }

    void HandleDash(Vector2 input)
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !_isDashing && _dashCD <= 0)
        {
            if (controller.collsionInfo.below || (ms.allowAirDash && !_hasDashed))
            {
                _isDashing = true;
                _dashTimeLeft = ms.dashTime;
                _dashCD = ms.dashCooldown;
                _dashDir = (input.x != 0) ? (int)Mathf.Sign(input.x) : controller.collsionInfo.faceDir;
                _hasDashed = true;
            }
        }

        if (_isDashing)
        {
            _velocity.y = 0;
            _velocity.x = _dashDir * ms.dashSpeed;
            _dashTimeLeft -= Time.deltaTime;
            if (_dashTimeLeft <= 0)
                _isDashing = false;
        }

        if (_dashCD > 0) _dashCD -= Time.deltaTime;
        if (controller.collsionInfo.below) _hasDashed = false;
    }
    void HorizontalMovement(Vector2 input, int wallDirX)
    {
        float targetVelocityX = input.x * ms.moveSpeed;
        _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref velocityXSmoothing,(controller.collsionInfo.below) ? ms.accerlationTimeGrounded : ms.accerlationTimeAir);
    }
    void WallSliding(Vector2 input, int wallDirX)
    {
        _isWallSliding = false;
        if ((controller.collsionInfo.left || controller.collsionInfo.right) &&
            !controller.collsionInfo.below && _velocity.y < 0)
        {
            _isWallSliding = true;

            if (_velocity.y < -ms.wallSideSpeedMax)
                _velocity.y = -ms.wallSideSpeedMax;

            if (ms.timeToWallUnstick > 0)
            {
                _velocity.x = 0;
                velocityXSmoothing = 0;
                if (input.x != wallDirX && input.x != 0)
                    ms.timeToWallUnstick -= Time.deltaTime;
                else
                    ms.timeToWallUnstick = ms.wallStickTime;
            }
            else
            {
                ms.timeToWallUnstick = ms.wallStickTime;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && _isWallSliding)
        {
            if (wallDirX == input.x)
            {
                _velocity.x = -wallDirX * ms.wallJumpClimb.x;
                _velocity.y = ms.wallJumpClimb.y;
            }
            else if (input.x == 0)
            {
                _velocity.x = -wallDirX * ms.wallJumpOff.x;
                _velocity.y = ms.wallJumpOff.y;
            }
            else
            {
                _velocity.x = -wallDirX * ms.wallLeap.x;
                _velocity.y = ms.wallLeap.y;
            }
            _justWallJumped = true;
            _inputTimer = 0;
        }
    }
    void Jumping()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //buffer
            _inputTimer = inputBuffer;
        }

        if (_inputTimer > 0)
            //buffer cd going down
            _inputTimer -= Time.deltaTime;

        if (!_justWallJumped && (_coyoteTimer > 0 || controller.collsionInfo.below) && _inputTimer > 0)
        {
            //resetting stuff ? i dont memeber
            _velocity.y = maxJumpVelocity;
            _inputTimer = 0;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            //jumping
            if (_velocity.y > minJumpVelocity)
                _velocity.y = minJumpVelocity;
        }

        if (controller.collsionInfo.below)
        {
            //setting coyote time
            _coyoteTimer = coyoteTime;
            _justWallJumped = false;
        }
        else
        {
            //coyote time going down 
            _coyoteTimer -= Time.deltaTime;
        }
    }
    void HandleGrapple()
    {
        if (currentGrappleArea == null)
            return;

        grapplePoint = currentGrappleArea.GetAttachPoint();
        ropeLength = Vector2.Distance(transform.position, grapplePoint);

        lineRenderer.enabled = true;

        totalVelocity = _velocity;
    }
    void ConstrainToRope()
    {
        Vector2 pos = transform.position;
        Vector2 toGrapple = pos - grapplePoint;
        float distance = toGrapple.magnitude;
        if (distance <= 0.0001f) return;

        Vector2 dir = toGrapple / distance;

        float radialSpeed = Vector2.Dot(totalVelocity, dir);
        totalVelocity -= dir * radialSpeed;

        Vector2 corrected = grapplePoint + dir * ropeLength;
        transform.position = new Vector3(corrected.x, corrected.y, transform.position.z);
    }

    void UpdateGrappleLine()
    {
        if (!lineRenderer.enabled)
            return;

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }
    void Gravity()
    {    

        if(_velocity.y < 0)
        {
           _velocity.y += (ms.gravity * Time.deltaTime) * ms.fallMultiplier;
           
        }
        else
        {
            _velocity.y += ms.gravity * Time.deltaTime;

        }
    }
    void StopBouncing()
    {
        if (controller.collsionInfo.above && _velocity.y > 0) _velocity.y = 0;
        if (controller.collsionInfo.below && _velocity.y < 0) _velocity.y = 0;
    }
    void CheckInteraction()
    {
        Vector2 origin = (controller.collsionInfo.faceDir == 1)
            ? controller.raycastOrigins.bottomRight
            : controller.raycastOrigins.bottomLeft;

        origin += Vector2.up * 1f;
        Vector2 direction = Vector2.right * controller.collsionInfo.faceDir;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, interactDistance, npcMask);
        Debug.DrawRay(origin, direction * interactDistance, Color.blue);

        if (hit)
        {
            npc_Interaction = hit.collider.GetComponent<NPC_interaction>();

            if (npc_Interaction != null && Input.GetKeyDown(KeyCode.E))
            {
                npc_Interaction.ActivateDialouge();
            }
        }
        else
        {
            npc_Interaction = null;
        }
    }
    private bool InDialogue()
    {
        return npc_Interaction != null && npc_Interaction.DialogueActive();
    }
    void UpdateFacing(Vector2 input)
    {
        if (input.x > 0)
            facingDir = 1;
        else if (input.x < 0)
            facingDir = -1;

        else
        {
            if (controller.collsionInfo.left)
                facingDir = 1;
            else if (controller.collsionInfo.right)
                facingDir = -1;
        }
    }
    void UpdateCameraDamping()
    {
        float dampingTransitionSpeed = 5f;
        float targetDamping;

        if (positionComposer == null) return;


        if (_isWallSliding)
        {
            // wall slide
            targetDamping = 0f;     
        }
        else if (!controller.collsionInfo.below && _velocity.y < -10f)
        {
            // fast fall
            targetDamping = 0f;      
        }
        else
        {
            // chillin
            targetDamping = 2f;      
        }

        positionComposer.Damping.y = Mathf.Lerp(positionComposer.Damping.y,targetDamping,dampingTransitionSpeed * Time.deltaTime);
    }
}
