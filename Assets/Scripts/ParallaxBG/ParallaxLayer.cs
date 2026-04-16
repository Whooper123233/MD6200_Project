using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private float parallaxFactor = 0.5f;

    public void Move(Vector2 delta)
    {
        transform.localPosition += new Vector3(
            delta.x * parallaxFactor,
            delta.y * parallaxFactor,
            0f
        );
    }
}
