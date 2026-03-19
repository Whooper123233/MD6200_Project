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

    [SerializeField] public float jumpHeight = 4;
    [SerializeField] public float timeToJumpApex = 2f;

    [SerializeField] public float jumpCutMultiplier = .01f;
    [SerializeField] public float fallMultiplier = 1.2f;

    [HideInInspector] public float gravity;
    [HideInInspector] public float jumpVelocity;
    private Vector3 _velocity;

    float targetVelocityX;
    float velocityXSmoothing;

    [SerializeField] float accerlationTimeAir = .1f;
    [SerializeField] float accerlationTimeGrounded = .5f;

    public float coyoteTime = .2f;
    private float _coyoteTimer;

    public float inputBuffer = .2f;
    private float _inputTimer;


    public Controller2D controller;
    void Start()
    {
        controller = GetComponent<Controller2D>();
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    void Update()
    {
        if (controller.collsionInfo.above || controller.collsionInfo.below) 
        {
            _velocity.y = 0;
        }
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
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
        if ((_coyoteTimer > 0 || controller.collsionInfo.below) && _inputTimer > 0)
        {
            _velocity.y = jumpVelocity;
            _inputTimer = 0;  
        }
        if (Input.GetKeyUp(KeyCode.Space) && _velocity.y > 0)
        {
            _velocity.y *= jumpCutMultiplier;
        }
        if (_velocity.y < 0 )
        {
            _velocity.y += gravity * (fallMultiplier - 1) * Time.deltaTime;
        }

        float accelerationTime = controller.collsionInfo.below ? accerlationTimeGrounded : accerlationTimeAir;
        targetVelocityX = input.x * moveSpeed;
        _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTime);

        _velocity.y += gravity * Time.deltaTime;
        controller.Move(_velocity *  Time.deltaTime);

       
    }

}
