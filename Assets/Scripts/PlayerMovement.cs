using UnityEngine;
using System.Collections;
[RequireComponent(typeof (Controller2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public float gravity = -28;
    private Vector3 _velocity;


    public Controller2D controller;
    void Start()
    {
        controller = GetComponent<Controller2D>();
    }

    void Update()
    {
        _velocity.y = gravity * Time.deltaTime;
        controller.Move(_velocity *  Time.deltaTime);
    }

    
}
