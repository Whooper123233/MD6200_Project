using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

public class Controller2D : MonoBehaviour
{
    [SerializeField] public float skinWidth = 0.015f;
    [SerializeField] public LayerMask collisionMask;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    public float maxClimbAngle = 80;
    public float maxDecendAngle = 75;

    public float horizontalRaySpacing;
    public float verticalRaySpacing;

    BoxCollider2D boxCollider;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collsionInfo;
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        CalcualteRaySpacing();
    }
    public void Move(Vector3 velocity )
    {
        UpdateRayCastOrigins();
        collsionInfo.Reset();
        if (velocity.y < 0)
        {
            DecendSlope(ref velocity);
        }
        if(velocity.x != 0)
        {
            HorizontalCollisions( ref velocity );
        }
        if (velocity.y != 0)
        {
            VerticalCollisions( ref velocity );
        }

        transform.Translate(velocity);
    }
    void HorizontalCollisions(ref Vector3 velocity)
    {
        float dirX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (dirX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i );
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * dirX * rayLength, Color.red);
            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(i == 0 && slopeAngle <= maxClimbAngle)
                {
                    float disranceToSlopeStart = 0f;
                    if (slopeAngle != collsionInfo.originSlopAngle) 
                    {
                        disranceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= disranceToSlopeStart * dirX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += disranceToSlopeStart * dirX;
                }
                if(!collsionInfo.climbingSlop || slopeAngle > maxClimbAngle)
                {

                    velocity.x = (hit.distance - skinWidth) * dirX;
                    rayLength = hit.distance;
                    if (collsionInfo.climbingSlop) 
                    {
                        velocity.y = Mathf.Tan(collsionInfo.slopAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }
                    collsionInfo.left = dirX == -1;
                    collsionInfo.right = dirX == 1;
                }
            }
        }
    }
    void VerticalCollisions(ref Vector3 velocity)
    {
        float dirY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (dirY == -1 ) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * dirY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * dirY * rayLength, Color.red);
            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * dirY;
                rayLength = hit.distance;

                if (collsionInfo.climbingSlop)
                {
                    velocity.x = velocity.y / Mathf.Tan(collsionInfo.slopAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collsionInfo.below = dirY == -1;
                collsionInfo.above = dirY == 1;
            }
        }
        if (collsionInfo.climbingSlop)
        {
            float dirX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = ((dirX == 1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin,Vector2.right * dirX ,rayLength,collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(slopeAngle != collsionInfo.slopAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * dirX;
                    collsionInfo.slopAngle = slopeAngle;
                }
            }
        }
    }
    void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        if (velocity.y <= climbVelocityY) 
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            collsionInfo.below = true;
            collsionInfo.climbingSlop = true;
            collsionInfo.slopAngle = slopeAngle;
        }
    }

    void DecendSlope(ref Vector3 velocity)
    {
        float dirX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (dirX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if(slopeAngle != 0 && slopeAngle <= maxDecendAngle)
            {
                if(Mathf.Sign(hit.normal.x) == dirX)
                {
                    if(hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) *Mathf.Abs(velocity.x)){
                        float moveDis = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDis;
                        velocity.x = Mathf.Cos (slopeAngle * Mathf .Deg2Rad) * moveDis * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;

                        collsionInfo.slopAngle = slopeAngle;
                        collsionInfo.descendingSlope = true;
                        collsionInfo.below = true;
                    }
                }
            }
        }
    }

    void UpdateRayCastOrigins()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }
    void CalcualteRaySpacing()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;
        public bool climbingSlop;
        public float slopAngle;
        public float originSlopAngle;
        public bool descendingSlope;
      
        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlop = false;
            descendingSlope = false;

            originSlopAngle = slopAngle;
        }
    }



}
    

