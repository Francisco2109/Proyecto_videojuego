using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Velocity")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Input Keys")]
    [SerializeField] private KeyCode leftKey;
    [SerializeField] private KeyCode rightKey;
    [SerializeField] private KeyCode jumpKey;

    [Header("Ground Detector")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb; 
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleJump();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float direction = 0f;

        if (Input.GetKey(leftKey))
            direction = -1f;
        else if (Input.GetKey(rightKey))
            direction = 1f;

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y); 
    }

    private void HandleJump()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        ); // check de que este tocando el piso bool

        if (isGrounded && Input.GetKeyDown(jumpKey))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); 
        }
    }
}