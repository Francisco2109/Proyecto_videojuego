using UnityEngine;
using System.Threading.Tasks;

public class TeleportAbility : MonoBehaviour
{
    // Definimos las direcciones posibles para el teletransporte
    public enum TeleportAxis
    {
        Vertical,
        Horizontal
    }

    [Header("Script Settings")]
    public TeleportAxis axis; // para unity UI elegir axis TP
    public LayerMask teleportLayer; // para unity UI elegir layer de bloques teleportables
    public KeyCode activationKey = KeyCode.E; // tecla para activar el TP
    public float offset = 0.1f; // distancia extra para evitar quedar atrapado en el bloque al teletransportarse
    private int cooldownMilliseconds = 500; // tiempo de cooldown entre teletransportes

    private bool isOnCooldown = false;
    private bool canTeleport = false; // flag para saber si el player esta en un bloque teleportable
    private Collider2D currentBlock; // referencia al bloque actual donde esta el player

    void Update()
    {
        if (canTeleport && !isOnCooldown && Input.GetKeyDown(activationKey))
        {
            Teleport(); // Accion de Habilidad de Teletransporte
            StartCooldown(); // Iniciar cooldown para evitar teletransportes consecutivos rápidos
        }
    }

    private void Teleport()
    {
        if (currentBlock == null) return;

        Bounds bounds = currentBlock.bounds; // obtenemos los limites del bloque para calcular la nueva posicion del player
        Vector3 newPosition = transform.position; // partimos de la posicion actual del player para modificar solo el eje correspondiente

        switch (axis)
        {
            case TeleportAxis.Vertical: // Agua / Water
                newPosition.y = (transform.position.y < bounds.center.y)
                    ? bounds.max.y + offset
                    : bounds.min.y - offset;
                break;

            case TeleportAxis.Horizontal: // Viento / Wind
                newPosition.x = (transform.position.x < bounds.center.x)
                    ? bounds.max.x + offset
                    : bounds.min.x - offset;
                break;
        }

        transform.position = newPosition; // Teletransportamos al player a la nueva posicion calculada
    }
    private async void StartCooldown()
    {
        isOnCooldown = true;
        await Task.Delay(cooldownMilliseconds); // Usa el tiempo de la compu no del juego, asi que no se ve afectado por Time.timeScale
        isOnCooldown = false;
    }
    // Detectamos cuando el player entra en un bloque teleportable para activar la posibilidad de teletransportarse, y guardamos una referencia al bloque actual para usarla en el calculo de la nueva posicion
    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((teleportLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            canTeleport = true;
            currentBlock = other;
        }
    }
    // Es importante resetear el estado de teletransporte al salir del bloque para evitar que el player pueda teletransportarse desde una posicion no valida
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == currentBlock)
        {
            canTeleport = false;
            currentBlock = null;
        }
    }
}