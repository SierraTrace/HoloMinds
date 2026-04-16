using UnityEngine;
using TMPro;

// Manager principal del minijuego VR "Ext'it"
// El jugador esquiva los abrazos de su ex girando la cabeza
// Sistema basado en Autoestima según GDD
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
    private const int MINIGAME_INDEX = 4;

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

    // Inicia una nueva partida
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

    // Actualiza el temporizador
    void UpdateTimer()
    {
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            EndGame(true);
        }

        UpdateTimerUI();
    }

    // Maneja el resultado de un obstáculo (Ex)
    void HandleObstacleResult(bool wasDodged)
    {
        if (!isGameActive) return;

        if (wasDodged)
        {
            // Esquiva exitosa: +1 autoestima
            totalDodges++;
            currentAutoestima += autoestimaPerDodge;
            
            if (currentAutoestima > maxAutoestimaReached)
            {
                maxAutoestimaReached = currentAutoestima;
            }

            Debug.Log($"VRGameManager: ¡Esquivas al Ex! Autoestima: {currentAutoestima}");
        }
        else
        {
            // Abrazo del Ex: -2 autoestima
            totalHits++;
            currentAutoestima -= autoestimaPerHit;

            Debug.Log($"VRGameManager: ¡Tu Ex te abraza! Autoestima: {currentAutoestima}");

            // Comprueba derrota: autoestima <= 0
            if (currentAutoestima <= 0)
            {
                currentAutoestima = 0;
                EndGame(false);
                return;
            }
        }

        UpdateUI();
    }

    // Actualiza toda la UI
    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Autoestima: {currentAutoestima}";
        }

        if (comboText != null)
        {
            comboText.text = $"Esquivas: {totalDodges}";
        }

        UpdateTimerUI();
    }

    // Actualiza solo el timer
    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    // Termina el juego
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
            string victoryReason = isVictory ? "Sobrevives al tiempo" : "Tu autoestima llega a 0";
            
            finalScoreText.text = $"{resultMessage}\n" +
                                  $"{victoryReason}\n\n" +
                                  $"Autoestima final: {currentAutoestima}\n" +
                                  $"Máxima alcanzada: {maxAutoestimaReached}\n" +
                                  $"Esquivas: {totalDodges}\n" +
                                  $"Abrazos recibidos: {totalHits}";
        }

        int finalScore = maxAutoestimaReached * 10;
        SaveScoreToGlobalManager(finalScore);

        Debug.Log($"VRGameManager: Juego terminado - {(isVictory ? "Victoria" : "Derrota")} - Score: {finalScore}");
    }

    // Guarda la puntuación en el GameManager global
    void SaveScoreToGlobalManager(int score)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(MINIGAME_INDEX, score);
        }
    }

    // Continúa al siguiente nivel
    public void ContinueToNextLevel()
    {
        SceneLoader.LoadNextScene();
    }

    // Reinicia el minijuego
    public void RestartGame()
    {
        StartGame();
    }

    // Vuelve al menú principal
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
