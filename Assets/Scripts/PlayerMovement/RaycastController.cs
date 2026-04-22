// -----------------------------------------------------------------------------
// File: [RaycastController]
// Author: [Veronica Wong]
// Contributors: []
// Created: [19/03/26]
// Description: [Raycasts to use for collision detection]
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

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    [SerializeField] public LayerMask collisionMask;

    [HideInInspector] public float horizontalRaySpacing;
    [HideInInspector] public float verticalRaySpacing;

    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    [SerializeField] public float skinWidth = 0.015f;

    [HideInInspector] public BoxCollider2D boxCollider;
    [HideInInspector] public RaycastOrigins raycastOrigins;

    public virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        CalcualteRaySpacing();
    }
    public void UpdateRayCastOrigins()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }
    public void CalcualteRaySpacing()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
