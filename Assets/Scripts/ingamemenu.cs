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
    [SerializeField] private GameObject gameOverCanvas;

    [Header("Victory")]
    [SerializeField] private GameObject victoryCanvas;

    [Header("Timer")]
    [SerializeField] private float timeLimit = 121f;
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

    public bool IsGameEnded => gameEnded;

    // ==================== INICIALIZACIÓN ====================
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        currentTime = timeLimit;
        currentLives = maxLives;
        player1InGoal = false;
        player2InGoal = false;

        if (pauseCanvas != null)
            pauseCanvas.enabled = false;

        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(false);

        if (victoryCanvas != null)
            victoryCanvas.SetActive(false);
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


        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayLose();

        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(true);

       // Time.timeScale = 0f;
    }

    // ==================== PAUSE ====================
    public void TogglePause()
    {
        if (isPaused) { 
            pauseCanvas.gameObject.SetActive(false);
            ResumeGame(); 
            }
        else { 
            pauseCanvas.gameObject.SetActive(true);
            PauseGame(); 
            }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;

        if (pauseCanvas != null)
            pauseCanvas.enabled = true;
    }

    public void StopSounds(){
        
        SoundManager.Instance.StopAllSounds();
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

        if (SoundManager.Instance != null)
            SoundManager.Instance.StopAllSounds();

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
    public void SetPlayerInGoal(PlayerVictory.PlayerSlot playerSlot, bool isInGoal)
    {
        if (gameEnded)
            return;

        switch (playerSlot)
        {
            case PlayerVictory.PlayerSlot.Player1:
                player1InGoal = isInGoal;
                break;
            case PlayerVictory.PlayerSlot.Player2:
                player2InGoal = isInGoal;
                break;
        }

        CheckVictoryConditions();
    }

    public void CheckVictoryConditions()
    {
        if (gameEnded)
            return;

        if (player1InGoal && player2InGoal)
        {
            Victory();
        }
    }

    private void Victory()
    {
        gameEnded = true;
        Time.timeScale = 0f;


        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayVictory();

        if (victoryCanvas != null)
            victoryCanvas.SetActive(true);

        //Time.timeScale = 0f;
    }
}