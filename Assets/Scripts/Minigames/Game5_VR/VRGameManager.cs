using UnityEngine;
using TMPro;

/// <summary>
/// Manager principal del minijuego VR "Ext'it".
/// El jugador debe esquivar los abrazos de su ex girando la cabeza.
/// Sistema basado en Autoestima según GDD.
/// </summary>
public class VRGameManager : MonoBehaviour
{
    [Header("Referencias")]
    public ObstacleSpawner obstacleSpawner;
    
    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI comboText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    [Header("Configuración del Juego (GDD)")]
    [Tooltip("Duración del minijuego en segundos (máx 60s según GDD)")]
    public float gameDuration = 45f;
    
    [Tooltip("Autoestima inicial")]
    public int startingAutoestima = 10;

    [Tooltip("Autoestima ganada por esquiva (GDD: +1)")]
    public int autoestimaPerDodge = 1;
    
    [Tooltip("Autoestima perdida por impacto (GDD: -2)")]
    public int autoestimaPerHit = 2;

    [Header("Estado del Juego")]
    [SerializeField] private int currentAutoestima = 0;
    [SerializeField] private int maxAutoestimaReached = 0;
    [SerializeField] private float timeRemaining;
    [SerializeField] private bool isGameActive = false;
    [SerializeField] private int totalDodges = 0;
    [SerializeField] private int totalHits = 0;

    // Índice de este minijuego para el GameManager global
    private const int MINIGAME_INDEX = 4; // Game5 = índice 4 (0-indexed)

    void Start()
    {
        if (obstacleSpawner != null)
        {
            obstacleSpawner.OnObstacleResult += HandleObstacleResult;
        }
        StartGame();
    }

    void Update()
    {
        if (isGameActive)
        {
            UpdateTimer();
        }
    }

    /// <summary>
    /// Inicia una nueva partida
    /// </summary>
    public void StartGame()
    {
        currentAutoestima = startingAutoestima;
        maxAutoestimaReached = startingAutoestima;
        totalDodges = 0;
        totalHits = 0;
        timeRemaining = gameDuration;
        isGameActive = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (obstacleSpawner != null)
        {
            obstacleSpawner.ResetSpawner();
            obstacleSpawner.StartSpawning();
        }

        UpdateUI();
        Debug.Log("VRGameManager: ¡Ext'it iniciado! Esquiva a tu Ex girando la cabeza.");
    }

    void UpdateTimer()
    {
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            EndGame(true); // Victoria por tiempo
        }

        UpdateTimerUI();
    }

    /// <summary>
    /// Maneja el resultado de un obstáculo (Ex)
    /// </summary>
    void HandleObstacleResult(bool wasDodged)
    {
        if (!isGameActive) return;

        if (wasDodged)
        {
            // ¡Esquivado! +1 autoestima según GDD
            totalDodges++;
            currentAutoestima += autoestimaPerDodge;
            
            // Trackear máximo alcanzado
            if (currentAutoestima > maxAutoestimaReached)
            {
                maxAutoestimaReached = currentAutoestima;
            }

            Debug.Log($"VRGameManager: ¡Esquivaste al Ex! Autoestima: {currentAutoestima}");
        }
        else
        {
            // Impacto (abrazo del Ex) -2 autoestima según GDD
            totalHits++;
            currentAutoestima -= autoestimaPerHit;

            Debug.Log($"VRGameManager: ¡Tu Ex te abrazó! Autoestima: {currentAutoestima}");

            // Comprobar derrota: autoestima <= 0
            if (currentAutoestima <= 0)
            {
                currentAutoestima = 0;
                EndGame(false); // Derrota por autoestima
                return;
            }
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Autoestima: {currentAutoestima}";
        }

        if (comboText != null)
        {
            // Mostrar estadísticas de esquivas
            comboText.text = $"Esquivas: {totalDodges}";
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    /// <summary>
    /// Termina el juego
    /// </summary>
    void EndGame(bool isVictory)
    {
        isGameActive = false;

        if (obstacleSpawner != null)
        {
            obstacleSpawner.StopSpawning();
            obstacleSpawner.ClearAllObstacles();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            string resultMessage = isVictory ? "¡VICTORIA!" : "DERROTA";
            string victoryReason = isVictory ? "Sobreviviste al tiempo" : "Tu autoestima llegó a 0";
            
            finalScoreText.text = $"{resultMessage}\n" +
                                  $"{victoryReason}\n\n" +
                                  $"Autoestima final: {currentAutoestima}\n" +
                                  $"Máxima alcanzada: {maxAutoestimaReached}\n" +
                                  $"Esquivas: {totalDodges}\n" +
                                  $"Abrazos recibidos: {totalHits}";
        }

        // Calcular puntuación final para el GameManager global
        // Usamos la autoestima máxima alcanzada * 10 para tener valores comparables
        int finalScore = maxAutoestimaReached * 10;
        SaveScoreToGlobalManager(finalScore);

        Debug.Log($"VRGameManager: Juego terminado - {(isVictory ? "Victoria" : "Derrota")} - Score: {finalScore}");
    }

    void SaveScoreToGlobalManager(int score)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(MINIGAME_INDEX, score);
        }
    }

    public void ContinueToNextLevel()
    {
        SceneLoader.LoadNextScene();
    }

    public void RestartGame()
    {
        StartGame();
    }

    public void ReturnToMainMenu()
    {
        SceneLoader.LoadSceneByName("MainMenu");
    }

    void OnDestroy()
    {
        if (obstacleSpawner != null)
        {
            obstacleSpawner.OnObstacleResult -= HandleObstacleResult;
        }
    }
}
