using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;    // Singleton instance

    [Header("Configuration")]
    public TextMeshProUGUI scoreText;
    public float gameSpeed = 5f;
    public float gameDuration = 30f;        // Duración total del minijuego

    private float _currentDistance;
    private float _timer;
    private bool _isPaused = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _timer = gameDuration;

        float maxScore = gameSpeed * gameDuration;

        // Establecer el valor máximo en la barra de autoestima
        if (SelfEsteemBar.Instance != null)
        {
            SelfEsteemBar.Instance.SetMaxValue(maxScore);
        }
    }

    public float GetWorldSpeed()
    {
        return _isPaused ? 0f : gameSpeed;
    }

    void Update()
    {
        if (_isPaused) return;

        _timer -= Time.deltaTime;

        if (_timer < 0f)
        {
            _timer = 0f;
            StopScore();
            SendFinalScore();
            return;
        }

        // Calcular distancia
        _currentDistance += gameSpeed * Time.deltaTime;

        if (scoreText != null)
        {
            scoreText.text = Mathf.FloorToInt(_currentDistance).ToString() + " m";
        }

        if (SelfEsteemBar.Instance != null)
        {
            SelfEsteemBar.Instance.SetValue(_currentDistance);
        }
    }

    public int GetFinalScore() => Mathf.FloorToInt(_currentDistance);

    public void StopScore() => _isPaused = true;

    private void SendFinalScore()
    {
        int finalScore = GetFinalScore();

        MinigameEnd end = FindObjectOfType<MinigameEnd>();
        if (end != null)
        {
            end.FinishMinigameWithScore(finalScore);
        }
    }

}
