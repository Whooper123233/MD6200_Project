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
[RequireComponent(typeof(Controller2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Jumping")]
    [SerializeField] public float moveSpeed = 5;
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    [SerializeField] public float timeToJumpApex = 2f;
    [SerializeField] public float jumpCutMultiplier = .01f;
    [SerializeField] public float fallMultiplier = 1.2f;
    [HideInInspector] public float gravity;


    float maxJumpVelocity;
    float minJumpVelocity;
    public Vector3 _velocity;
    float targetVelocityX;
    float velocityXSmoothing;

    [Header("Jump Height")]
    [SerializeField] float accerlationTimeAir = .1f;
    [SerializeField] float accerlationTimeGrounded = .5f;

    [Header("Wall Jumping")]
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    [SerializeField] float wallSideSpeedMax = 3.2f;
    public float wallStickTime = .25f;
    public float timeToWallUnstick;
    private bool _justWallJumped = false;

    [Header("Coyote Time & Input Buffer")]
    public float coyoteTime = .2f;
    private float _coyoteTimer;
    public float inputBuffer = .2f;
    private float _inputTimer;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 0.5f;
    [SerializeField] private bool allowAirDash = true;
    private bool _isDashing;
    private float _dashTimeLeft;
    private float _dashCD;
    private int _dashDir;
    private bool _hasDashed;

    [Header("Grapple")]
    public GrappleSwing grapple;
    public Controller2D controller;


    void Start()
    {
        
        controller = GetComponent<Controller2D>();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

    }

    void Update()
    {

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        int wallDirX = (controller.collsionInfo.left) ? -1 : 1;

        HandleDash(input);
        HorizontalMovement(input, wallDirX);
        WallSliding(input, wallDirX);
        Jumping();
        if (!grapple.isSwinging)
        {          
            Gravity();
            controller.Move(_velocity * Time.deltaTime);
        }
        StopBouncing();
    }

    void HandleDash(Vector2 input)
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !_isDashing && _dashCD <= 0)
        {
            if (controller.collsionInfo.below || (allowAirDash && !_hasDashed))
            {
                _isDashing = true;
                _dashTimeLeft = dashTime;
                _dashCD = dashCooldown;
                _dashDir = (input.x != 0) ? (int)Mathf.Sign(input.x) : controller.collsionInfo.faceDir;
                _hasDashed = true;
            }
        }

        if (_isDashing)
        {
            _velocity.y = 0;
            _velocity.x = _dashDir * dashSpeed;
            _dashTimeLeft -= Time.deltaTime;
            if (_dashTimeLeft <= 0)
                _isDashing = false;
        }

        if (_dashCD > 0) _dashCD -= Time.deltaTime;
        if (controller.collsionInfo.below) _hasDashed = false;
    }

    void HorizontalMovement(Vector2 input, int wallDirX)
    {
        float targetVelocityX = input.x * moveSpeed;
        _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref velocityXSmoothing,(controller.collsionInfo.below) ? accerlationTimeGrounded : accerlationTimeAir);
    }

    void WallSliding(Vector2 input, int wallDirX)
    {
        bool wallSliding = false;
        if ((controller.collsionInfo.left || controller.collsionInfo.right) &&
            !controller.collsionInfo.below && _velocity.y < 0)
        {
            wallSliding = true;
            if (_velocity.y < -wallSideSpeedMax)
                _velocity.y = -wallSideSpeedMax;

            if (timeToWallUnstick > 0)
            {
                _velocity.x = 0;
                velocityXSmoothing = 0;
                if (input.x != wallDirX && input.x != 0)
                    timeToWallUnstick -= Time.deltaTime;
                else
                    timeToWallUnstick = wallStickTime;
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && wallSliding)
        {
            if (wallDirX == input.x)
            {
                _velocity.x = -wallDirX * wallJumpClimb.x;
                _velocity.y = wallJumpClimb.y;
            }
            else if (input.x == 0)
            {
                _velocity.x = -wallDirX * wallJumpOff.x;
                _velocity.y = wallJumpOff.y;
            }
            else
            {
                _velocity.x = -wallDirX * wallLeap.x;
                _velocity.y = wallLeap.y;
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

    void Gravity()
    {    
        _velocity.y += gravity * Time.deltaTime;
    }

    void StopBouncing()
    {
        if (controller.collsionInfo.above && _velocity.y > 0) _velocity.y = 0;
        if (controller.collsionInfo.below && _velocity.y < 0) _velocity.y = 0;
    }

}
