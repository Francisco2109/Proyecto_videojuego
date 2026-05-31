// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class GameManager : MonoBehaviour
// {
//     public static GameManager Instance;

//     [Header("Keys")]
//     [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
//     [SerializeField] private KeyCode restartKey = KeyCode.R;

//     [Header("UI")]
//     [SerializeField] private Canvas pauseCanvas;

//     private bool isPaused = false;

//     private void Awake() // Aseguramos que solo exista una instancia del GameManager
//     {
//         if (Instance == null)
//             Instance = this;
//         else
//             Destroy(gameObject);
//     }

//     private void Update()
//     {
//         if (Input.GetKeyDown(pauseKey))
//         {
//             TogglePause();
//         }

//         if (Input.GetKeyDown(restartKey) && isPaused) // Solo permitir reiniciar el nivel si el juego está pausado para evitar reinicios accidentales durante el juego
//         {
//             RestartLevel();
//         }
//     }

//     public void TogglePause()
//     {
//         if (isPaused)
//             ResumeGame();
//         else
//             PauseGame();
//     }

//     public void PauseGame()
//     {
//         Time.timeScale = 0f; // Detiene el tiempo del juego
//         isPaused = true;
        
//         if (pauseCanvas != null)
//             pauseCanvas.enabled = true; // Muestra el canvas de pausa
//     }

//     public void ResumeGame()
//     {
//         Time.timeScale = 1f; // Reanuda el tiempo del juego
//         isPaused = false;
        
//         if (pauseCanvas != null)
//             pauseCanvas.enabled = false; // Oculta el canvas de pausa
//     }

//     public void RestartLevel()
//     {
//         Time.timeScale = 1f; // importante por si estaba pausado
//         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // recarga la escena actual para reiniciar el nivel
//     }
// }

////////// COMENTADO EL CODIGO ANTERIOR PARA INCLUIR LAS NUEVAS FUNCIONALIDADES DE TEMPORIZADOR, VIDAS Y GAME OVER //////////

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Keys")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] private KeyCode restartKey = KeyCode.R;

    [Header("Pause UI")]
    [SerializeField] private Canvas pauseCanvas;

    [Header("Game Over UI")]
    [SerializeField] private Canvas gameOverCanvas;

    [Header("Victory")]
    [SerializeField] private Canvas victoryCanvas;
    [SerializeField] private PlayerVictory waterGirlVictory;
    [SerializeField] private PlayerVictory windBoyVictory;

    [Header("Timer")]
    [SerializeField] private float timeLimit = 120f;
    [SerializeField] private TMP_Text timerText;

    [Header("Lives")]
    [SerializeField] private int maxLives = 3;
    [SerializeField] private TMP_Text livesText;

    private float currentTime;
    private int currentLives;
    private bool player1InGoal;
    private bool player2InGoal;
    private bool isPaused;
    private bool gameEnded;

    // ==================== INICIALIZACIÓN ====================
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        currentTime = timeLimit;
        currentLives = maxLives;

        if (pauseCanvas != null)
            pauseCanvas.enabled = false;

        if (gameOverCanvas != null)
            gameOverCanvas.enabled = false;

        UpdateUI();
    }

    // ==================== INPUT & CONTROL ====================
    private void Update()
    {
        if (!gameEnded)
        {
            UpdateTimer();
        }

        if (Input.GetKeyDown(pauseKey) && !gameEnded)
        {
            TogglePause();
        }

        if (Input.GetKeyDown(restartKey) && (isPaused || gameEnded))
        {
            RestartLevel();
        }
    }

    // ==================== Temporizador ====================
    private void UpdateTimer()
    {
        if (isPaused)
            return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            EndGame();
        }

        UpdateTimerUI();
    }

    // ==================== Vidas ====================
    public void LoseLife()
    {
        if (gameEnded)
            return;

        currentLives--;

        UpdateLivesUI();

        if (currentLives <= 0)
        {
            EndGame();
        }
    }

    // ==================== GAME STATE ====================
    private void EndGame()
    {
        gameEnded = true;

        Time.timeScale = 0f;

        if (gameOverCanvas != null)
            gameOverCanvas.enabled = true;
    }

    // ==================== PAUSE ====================
    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;

        if (pauseCanvas != null)
            pauseCanvas.enabled = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (pauseCanvas != null)
            pauseCanvas.enabled = false;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    // ==================== UI UPDATES ====================
    private void UpdateUI()
    {
        UpdateTimerUI();
        UpdateLivesUI();
    }

    private void UpdateTimerUI()
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void UpdateLivesUI()
    {
        if (livesText == null)
            return;

        livesText.text = $"Lives: {currentLives}";
    }

    // ==================== VICTORY ====================
    public void CheckVictoryConditions()
    {
        if (gameEnded)
            return;

        if (waterGirlVictory.IsPlayerInside() &&
            windBoyVictory.IsPlayerInside())
        {
            Victory();
        }
    }

    private void Victory()
    {
        gameEnded = true;

        Time.timeScale = 0f;

        if (victoryCanvas != null)
            victoryCanvas.enabled = true;
    }
}