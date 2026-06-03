using UnityEngine;

// Esta Funcion esta diseñada para aplicarse al objecto designado para ganar, 
// no asignar este script al jugador, sino al area de victoria.

public class PlayerVictory : MonoBehaviour
{
    public enum PlayerSlot
    {
        Player1,
        Player2
    }

    [SerializeField] private PlayerSlot playerSlot;
    [SerializeField] private GameObject targetPlayer; // El jugador que debe estar dentro de la zona para ganar

    private bool playerInside;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == targetPlayer)
        {
            playerInside = true; // Marca que el jugador está dentro de la zona de victoria

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetPlayerInGoal(playerSlot, true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == targetPlayer)
        {
            playerInside = false; // Marca que el jugador ha salido de la zona de victoria

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetPlayerInGoal(playerSlot, false);
            }
        }
    }

    public bool IsPlayerInside()
    {
        return playerInside; // Método público para que el GameManager pueda verificar si el jugador está dentro de la zona de victoria
    }
}