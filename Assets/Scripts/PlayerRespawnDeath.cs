using UnityEngine;

public class PlayerRespawnDeath : MonoBehaviour
{
    [Header("Respawn")]
    [SerializeField] private Transform spawnPoint; // Asignar el punto de respawn en el Inspector

    [Header("Death Layer")]
    [SerializeField] private LayerMask deathLayer; // configura este LayerMask en el Inspector para incluir los objetos que causan la muerte del jugador

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((deathLayer.value & (1 << collision.gameObject.layer)) != 0) // Verifica si el objeto con el que colisionamos está en el layer de muerte
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.Instance.LoseLife(); 

        transform.position = spawnPoint.position; // Teletransporta al jugador al punto de respawn

        Rigidbody2D rb = GetComponent<Rigidbody2D>(); // Reinicia la velocidad del jugador para evitar que siga moviéndose después de morir

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}