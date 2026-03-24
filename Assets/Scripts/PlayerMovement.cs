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
// Date last modified: []
// Last modified by: []
//
// -----------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
[RequireComponent(typeof (Controller2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5;

    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    [SerializeField] public float timeToJumpApex = 2f;

    [SerializeField] public float jumpCutMultiplier = .01f;
    [SerializeField] public float fallMultiplier = 1.2f;

    [HideInInspector] public float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    private Vector3 _velocity;

    float targetVelocityX;
    float velocityXSmoothing;

    [SerializeField] float accerlationTimeAir = .1f;
    [SerializeField] float accerlationTimeGrounded = .5f;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    [SerializeField] float wallSideSpeedMax = 3.2f;
    public float wallStickTime = .25f;
    public float timeToWallUnstick;
    private bool _justWallJumped = false;

    public float coyoteTime = .2f;
    private float _coyoteTimer;

    public float inputBuffer = .2f;
    private float _inputTimer;


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

        float targetVelocityX = input.x * moveSpeed;
        _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collsionInfo.below) ? accerlationTimeGrounded : accerlationTimeAir);
        bool wallSliding = false;

        CoyoteTimeInputBuffer();
        if ((controller.collsionInfo.left || controller.collsionInfo.right) && !controller.collsionInfo.below && _velocity.y < 0)
        {
        
            wallSliding = true;
            if (_velocity.y < -wallSideSpeedMax)
            {
                _velocity.y = -wallSideSpeedMax;
            }

            if (timeToWallUnstick > 0)
            {
                _velocity.x = 0;
                velocityXSmoothing = 0;

                if (input.x != wallDirX && input.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }
   
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _inputTimer = inputBuffer;

            if (wallSliding)
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
            if (controller.collsionInfo.below)
            {
                _velocity.y = maxJumpVelocity;
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (_velocity.y > minJumpVelocity)
            {
                _velocity.y = minJumpVelocity;
            }
        }

        _velocity.y += gravity * Time.deltaTime;   
        controller.Move(_velocity * Time.deltaTime);
        if (controller.collsionInfo.above && _velocity.y > 0)
        {
            _velocity.y = 0;
        }

        if (controller.collsionInfo.below && _velocity.y < 0)
        {
            _velocity.y = 0;
        }
    }

    public void CoyoteTimeInputBuffer()
    {

        if (controller.collsionInfo.below)
        {
            _coyoteTimer = coyoteTime;
        }
        else
        {
            _coyoteTimer -= Time.deltaTime;
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            _inputTimer = inputBuffer;
        }
        if (!_justWallJumped && (_coyoteTimer > 0 || controller.collsionInfo.below) && _inputTimer > 0)
        {
            _velocity.y = maxJumpVelocity;
            _inputTimer = 0;
        }
    }

}
