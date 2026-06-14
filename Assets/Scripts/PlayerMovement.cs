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
    public ParticleSystem jumpEffect; // Efecto de partículas para el salto

    [Header("Ground Detector")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    // =======================================================
    // NUEVAS VARIABLES PARA FORGIVENESS MECHANICS (COYOTE TIME & JUMP BUFFER)
    // =======================================================
    [Header("Forgiveness Mechanics")]
    [SerializeField] private float coyoteTimeDuration = 0.15f; // Cuánto tiempo extra tiene para saltar en el aire
    private float coyoteTimeCounter;                           // Contador interno para el Coyote Time

    [SerializeField] private float jumpBufferDuration = 0.15f; // Cuánto tiempo antes del suelo se puede presionar saltar
    private float jumpBufferCounter;                           // Contador interno para el Jump Buffer
    // =======================================================

    private Rigidbody2D rb; 
    private bool isGrounded;
    
    // --- VARIABLES PARA ANIMACIÓN ---
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded)
            return;

        // Primero detectamos si está en el suelo antes de manejar los tiempos
        CheckGround(); 
        
        HandleForgivenessTimers(); // Actualiza los contadores de tiempo de perdón
        HandleJump();
        UpdateWalkingSound();
        UpdateAnimator(); 
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded)
            return;

        HandleMovement();
    }

    private void CheckGround()
    {
        // Detecta si el personaje está tocando el suelo
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private void HandleForgivenessTimers()
    {
        // --- LÓGICA DE COYOTE TIME ---
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTimeDuration; // Si está en el suelo, el contador se reinicia al máximo
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime; // Si está en el aire, el tiempo empieza a correr hacia atrás
        }

        // --- LÓGICA DE JUMP BUFFER ---
        if (Input.GetKeyDown(jumpKey))
        {
            jumpBufferCounter = jumpBufferDuration; // Si presionas saltar, guardamos la intención asignando el tiempo máximo
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime; // El valor va disminuyendo con el tiempo
        }
    }

    private void HandleMovement()
    {
        float direction = 0f;

        if (Input.GetKey(leftKey))
        {
            direction = -1f;
            if (spriteRenderer != null) spriteRenderer.flipX = true; 
        }
        else if (Input.GetKey(rightKey))
        {
            direction = 1f;
            if (spriteRenderer != null) spriteRenderer.flipX = false; 
        }

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y); 
    }

    private void HandleJump()
    {
        // MODIFICACIÓN CRUCIAL:
        // En lugar de "si está en el suelo Y presiona saltar justo ahora", verificamos:
        // ¿Tiene tiempo de Coyote disponible? (coyoteTimeCounter > 0) Y ¿Presionó saltar hace poquito? (jumpBufferCounter > 0)
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); 

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayJump();
            }

            if (jumpEffect != null)
            {
                jumpEffect.Play();
            }
            // Gastamos de inmediato los contadores para evitar que salte infinitamente en el aire
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }
    }

    private void UpdateAnimator()
    {
        if (anim == null) return;

        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetBool("IsGrounded", isGrounded);
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