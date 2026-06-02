using UnityEngine;

public class PlayerRespawnDeath : MonoBehaviour
{
    [Header("Respawn")]
    [SerializeField] private Transform spawnPoint; 

    [Header("Death Layer")]
    [SerializeField] private LayerMask deathLayer; 

    // --- NUEVA VARIABLE PARA ANIMACIÓN ---
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((deathLayer.value & (1 << collision.gameObject.layer)) != 0) 
        {
            Die();
        }
    }

    private void Die()
    {
        // 1. Activamos la animación de muerte de inmediato
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        GameManager.Instance.LoseLife(); 

        transform.position = spawnPoint.position; 

        Rigidbody2D rb = GetComponent<Rigidbody2D>(); 

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}