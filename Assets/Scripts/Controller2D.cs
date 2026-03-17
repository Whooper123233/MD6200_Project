using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

public class Controller2D : MonoBehaviour
{
    [SerializeField] public float skinWidth = 0.015f;
    [SerializeField] public LayerMask collisionMask;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

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
                velocity.x = (hit.distance - skinWidth) * dirX;
                rayLength = hit.distance;

                collsionInfo.left = dirX == -1;
                collsionInfo.right = dirX == 1;
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

                collsionInfo.below = dirY == -1;
                collsionInfo.above = dirY == 1;
            }
        }
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

        public void Reset()
        {
            above = below = false;
            left = right = false;
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



}
    

