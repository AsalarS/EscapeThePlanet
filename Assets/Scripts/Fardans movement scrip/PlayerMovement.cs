using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform orientation;

    [Header("Movement")]
    [SerializeField] private float speed = 40f;
    [SerializeField] private float sprintSpeedMultiplier = 1.5f;

    [Header("Drag")]
    [SerializeField] private float drag = 6f;

    private Vector3 moveDirection;

    private float horizontalInput;
    private float verticalInput;

    private bool isJumping = false;
    [SerializeField] private float jumpForce = 9.8f;
    [SerializeField] private float fallMultiplier = 2.5f; // Adjust the fall speed

    private void Start() => rb.freezeRotation = true;

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        moveDirection = (orientation.forward * verticalInput + orientation.right * horizontalInput).normalized;

        rb.drag = drag;

        // Sprinting
        float currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= sprintSpeedMultiplier;
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
        }
        else if (!Input.GetKey(KeyCode.Space) && isJumping)
        {
            isJumping = false;
        }

        // Adjust fall speed
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        rb.AddForce(moveDirection * currentSpeed, ForceMode.Acceleration);
    }

    void OnTriggerEnter(Collider theCollision) { 
        if (theCollision.gameObject.tag == "floor")
        {
            isJumping = false;
        } else
        {
            isJumping = true;
        }
    }
}