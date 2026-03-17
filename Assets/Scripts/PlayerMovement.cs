using UnityEngine;
using System.Collections;
[RequireComponent(typeof (Controller2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5;

    [SerializeField] public float jumpHeight = 4;
    [SerializeField] public float timeToJumpApex = 2f;

    [SerializeField] public float jumpCutMultiplier = 0.01f;
    [SerializeField] public float fallMultiplier = 1.2f;

    public float gravity;
    public float jumpVelocity;
    private Vector3 _velocity;

    float targetVelocityX;
    float velocityXSmoothing;
    [SerializeField] float accerlationTimeAir = .1f;
    [SerializeField] float accerlationTimeGrounded = .5f;


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

        if (Input.GetKeyDown(KeyCode.Space) && controller.collsionInfo.below)
        {
             _velocity.y = jumpVelocity;
        }
        if (Input.GetKeyUp(KeyCode.Space) && _velocity.y > 0)
        {
            _velocity.y *= jumpCutMultiplier;
        }
        if (_velocity.y < 0)
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
