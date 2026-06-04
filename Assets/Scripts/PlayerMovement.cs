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
    
    // --- VARIABLES PARA ANIMACIÓN ---
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Buscamos los componentes necesarios en el mismo objeto
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded)
            return;

        HandleJump();
        UpdateWalkingSound();
        UpdateAnimator(); // <-- Actualización de animaciones
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded)
            return;

        HandleMovement();
    }

    private void HandleMovement()
    {
        float direction = 0f;

        if (Input.GetKey(leftKey))
        {
            direction = -1f;
            if (spriteRenderer != null) spriteRenderer.flipX = true; // Mira a la izquierda
        }
        else if (Input.GetKey(rightKey))
        {
            direction = 1f;
            if (spriteRenderer != null) spriteRenderer.flipX = false; // Mira a la derecha
        }

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y); 
    }

    private void HandleJump()
    {
        // Detecta si el personaje está tocando el suelo
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        ); 

        // Solo permite saltar si está en el suelo (isGrounded es verdadero)
        if (isGrounded && Input.GetKeyDown(jumpKey))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); 

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayJump();
            }
        }
    }

    // --- MÉTODO PARA ENVIAR DATOS AL ANIMATOR ---
    private void UpdateAnimator()
    {
        if (anim == null) return;

        // 1. Envía la velocidad horizontal en valor absoluto (siempre positivo)
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));

        // 2. Envía el estado del suelo
        anim.SetBool("IsGrounded", isGrounded);

        // 3. Envía la velocidad vertical (positivo al subir, negativo al bajar)
        anim.SetFloat("VerticalVelocity", rb.linearVelocity.y);
    }

    private void UpdateWalkingSound()
    {
        if (SoundManager.Instance == null)
            return;

        bool isWalking = isGrounded && Mathf.Abs(rb.linearVelocity.x) > 0.01f;
        SoundManager.Instance.SetWalking(gameObject, isWalking);
    }
}