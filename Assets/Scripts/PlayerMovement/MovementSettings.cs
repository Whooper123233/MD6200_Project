using UnityEngine;

[CreateAssetMenu(menuName = "Settings")]
public class MovementSettings : ScriptableObject
{   
    [Header("Jumping")]
    [SerializeField] public float moveSpeed = 5;
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    [SerializeField] public float timeToJumpApex = 2f;
    [SerializeField] public float jumpCutMultiplier = .01f;
    [SerializeField] public float fallMultiplier = 1.2f;
    [HideInInspector] public float gravity;

    [Header("Jump Height")]
    [SerializeField] public float accerlationTimeAir = .1f;
    [SerializeField] public float accerlationTimeGrounded = .5f;

    [Header("Wall Jumping")]
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    [SerializeField] public float wallSideSpeedMax = 3.2f;
    public float wallStickTime = .25f;
    public float timeToWallUnstick;

    [Header("Dash")]
    [SerializeField] public float dashSpeed = 20f;
    [SerializeField] public float dashTime = 0.2f;
    [SerializeField] public float dashCooldown = 0.5f;
    [SerializeField] public bool allowAirDash = true;

    [SerializeField] public float swingAcceleration = 20f;
    [SerializeField] public float maxSwingSpeed = 15f;     
}
