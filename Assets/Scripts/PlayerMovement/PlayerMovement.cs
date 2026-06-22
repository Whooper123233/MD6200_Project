// -----------------------------------------------------------------------------
// File: [PlayerMovement]
// Author: [Veronica Wong]
// Contributors: []
// Created: [18/03/26]
// Description: [Player Movement]
// 
// Unity Version: [6000.3.2f1]
// Project: [Null_Error]
// 
// Date last modified: [26/03/26]
// Last modified by: []
//
// -----------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Assertions.Must;

[RequireComponent(typeof(Controller2D),typeof(LineRenderer))]
public class PlayerMovement : MonoBehaviour
{
    public MovementSettings ms;
    private NPC_interaction npc_Interaction;

    private bool _justWallJumped = false;

    [Header("Coyote Time & Input Buffer")]
    public float coyoteTime = .2f;
    private float _coyoteTimer;
    public float inputBuffer = .2f;
    private float _inputTimer;

    [Header("Interaction")]
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private LayerMask npcMask;

    private bool _isDashing;
    public float _dashTimeLeft;
    public float _dashCD;
    public int _dashDir;
    public bool _hasDashed;
    public Vector3 _velocity;
    float targetVelocityX;
    float velocityXSmoothing;
    float maxJumpVelocity;
    float minJumpVelocity;

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

    void Start()
    {     
        controller = GetComponent<Controller2D>();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;

        ms.gravity = -(2 * ms.maxJumpHeight) / Mathf.Pow(ms.timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(ms.gravity) * ms.timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(ms.gravity) * ms.minJumpHeight);
    }

    void Update()
    {
        if (InDialogue())
        {
            return;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        int wallDirX = (controller.collsionInfo.left) ? -1 : 1;

        HandleDash(input);
        HorizontalMovement(input, wallDirX);
        WallSliding(input, wallDirX);
        Jumping();
        if (Input.GetMouseButtonDown(0))
        {
            HandleGrapple();
        }

        UpdateGrappleLine();

        if (!isSwinging)
        {          
            Gravity();
            controller.Move(_velocity * Time.deltaTime);
        }
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
        bool wallSliding = false;
        if ((controller.collsionInfo.left || controller.collsionInfo.right) &&
            !controller.collsionInfo.below && _velocity.y < 0)
        {
            wallSliding = true;
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

        if (Input.GetKeyDown(KeyCode.Space) && wallSliding)
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
            _inputTimer = inputBuffer;
        }

        if (_inputTimer > 0)
            _inputTimer -= Time.deltaTime;

        if (!_justWallJumped && (_coyoteTimer > 0 || controller.collsionInfo.below) && _inputTimer > 0)
        {
            _velocity.y = maxJumpVelocity;
            _inputTimer = 0;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (_velocity.y > minJumpVelocity)
                _velocity.y = minJumpVelocity;
        }

        if (controller.collsionInfo.below)
        {
            _coyoteTimer = coyoteTime;
            _justWallJumped = false;
        }
        else
        {
            _coyoteTimer -= Time.deltaTime;
        }
    }
    void HandleGrapple()
    {
        if (currentGrappleArea == null)
            return;

        grapplePoint = currentGrappleArea.GetAttachPoint();

        lineRenderer.enabled = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, overlapRadius, grappleMask);
        foreach (Collider2D hit in hits)
        {
            Vector2 grapplePoint = hit.transform.position;
            Vector2 toGrapple = grapplePoint - (Vector2)transform.position;
            float radius = toGrapple.magnitude;

            RaycastHit2D rayHit = Physics2D.Raycast(transform.position, (hit.transform.position - transform.position).normalized);
            if (rayHit)
            {
                if ((rayHit.transform.gameObject.layer & wallMask.value) != 0)
                {
                    continue;
                }

                Vector2 dir = toGrapple.normalized;
                Vector2 tangent = new Vector2(-dir.y, dir.x);
                float tangentialVelocity = Vector2.Dot(_velocity,tangent);
                float angularVelocity = tangentialVelocity / radius;
                float centripetalForce = (angularVelocity * angularVelocity) * radius;
                Vector2 centripentalVelocity = -dir * centripetalForce;

                totalVelocity = centripentalVelocity + tangentialVelocity * tangent;

                return;
            }          
        }
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

}
