using UnityEngine;
using System;

public class PlayerMovementStates : MonoBehaviour
{
    public enum MoveState
    {
        idle,
        run,
        jump,
        wallJump,
        falling,
        wallSlide,
        dashing,
        swinging
    }

    public MoveState currentMoveState {  get; private set; }

    [SerializeField] private Animator animator;
    private const string idleAnim = "Idle";
    private const string runAnim = "Running";
    private const string jumpAnim = "Jumping";
    private const string wallJumpAnim = "WallJump";
    private const string fallAnim = "Falling";
    private const string wallSlideAnim = "WallSlide";
    private const string dashAnim = "Dashing";
    private const string swingAnim = "Swinging";


    public static Action<MoveState> OnPlayerMoveStateChanged;
    private PlayerMovement playerMovement;
    [SerializeField] public AudioManager audioManager;
    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }
    private void Update()
    {
        if (playerMovement._isDashing)
        {
            SetMoveState(MoveState.dashing);
        }
        else if (playerMovement._isWallSliding)
        {
            SetMoveState(MoveState.wallSlide);
        }
        else if (playerMovement._justWallJumped)
        {
            SetMoveState(MoveState.wallJump);
        }
        else if (playerMovement.isSwinging)
        {
            SetMoveState(MoveState.swinging);
        }
        else if (playerMovement._velocity.y > 0.01f)
        {
            SetMoveState(MoveState.jump);
        }
        else if (playerMovement._velocity.y < -0.01f)
        {
            SetMoveState(MoveState.falling);
        }
        else if (Mathf.Abs(playerMovement._velocity.x) > 0.01f)
        {
            SetMoveState(MoveState.run);
        }
        else
        {
            SetMoveState(MoveState.idle);
        }
    }
    public void SetMoveState(MoveState moveState)
    {
        if (moveState == currentMoveState) return;

        switch (moveState)
        {
            case MoveState.idle:
                HandleIdel();
                
                break;
            case MoveState.run:
                HandleRun();


                break;
            case MoveState.jump:
                HandleJump();

                break;
            case MoveState.wallJump:
                HandleWallJump();

                break;
            case MoveState.falling:
                HandleFalling();

                break;
            case MoveState.wallSlide:
                HandleWallSlide();

                break;
            case MoveState.dashing:
                HandleDashing();

                break;
            case MoveState.swinging:
                HandleSwinging();

                break;
            default:
                Debug.LogError($"Invalid Movestate: {moveState}");
                break;
        }
        OnPlayerMoveStateChanged?.Invoke( moveState );
        currentMoveState = moveState;
    }

    private void HandleIdel()
    {
        animator.Play(idleAnim);
        //Debug.Log("IDEL ANI PLAYING");
    }
    private void HandleRun()
    {
        animator.Play(runAnim);

        //Debug.Log("RUN ANI PLAYING");

    }
    private void HandleJump()
    {
        animator.Play(jumpAnim);
        //Debug.Log("JUMP ANI PLAYING");

    }
    private void HandleWallJump()
    {
        animator.Play(wallJumpAnim);
        //Debug.Log("WALL JUMP ANI PLAYING");

    }
    private void HandleWallSlide()
    {
        animator.Play(wallSlideAnim);
        //Debug.Log("WALL SLIDE ANI PLAYING");


    }
    private void HandleFalling()
    {
        animator.Play(fallAnim);
        //Debug.Log("FALL ANI PLAYING");


    }
    private void HandleDashing()
    {
        animator.Play(dashAnim);
        //Debug.Log("DASH ANI PLAYING");


    }
    private void HandleSwinging()
    {
        animator.Play(swingAnim);
        //Debug.Log("SWING ANI PLAYING");

    }
}
