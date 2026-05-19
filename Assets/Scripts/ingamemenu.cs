using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Keys")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] private KeyCode restartKey = KeyCode.R;

    private bool isPaused = false;

    private void Awake() // Aseguramos que solo exista una instancia del GameManager
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }

        if (Input.GetKeyDown(restartKey) && isPaused) // Solo permitir reiniciar el nivel si el juego está pausado para evitar reinicios accidentales durante el juego
        {
            RestartLevel();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f; // Detiene el tiempo del juego
        isPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // Reanuda el tiempo del juego
        isPaused = false;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; // importante por si estaba pausado
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // recarga la escena actual para reiniciar el nivel
    }
}