using UnityEngine;

public class ParallaxCamera : MonoBehaviour
{

    public delegate void ParallaxCameraDelegate(Vector2 deltaMovement);
    public event ParallaxCameraDelegate onCameraTranslate;

    private Vector3 oldPosition;

    void Start()
    {
        oldPosition = transform.position;
    }

    void Update()
    {
        Vector3 newPosition = transform.position;
        Vector3 delta = newPosition - oldPosition;

        if (delta.sqrMagnitude > 0.000001f)
        {
            onCameraTranslate?.Invoke(new Vector2(delta.x, delta.y));
            oldPosition = newPosition;
        }
    }
}
