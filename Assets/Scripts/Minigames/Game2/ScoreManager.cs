using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;    // Singleton instance

    [Header("Configuration")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public float gameSpeed = 5f;
    public float gameDuration = 30f;        // Duración total del minijuego

    [Header("Feedback Visual")]
    public Color warningcolor = Color.red;
    public float blinkInterval = 0.5f;

    private float _currentDistance;
    private float _timer;
    private bool _isPaused = false;
    private bool _isWarningActive = false;
    private Color _originalColor;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _timer = gameDuration;

        if (timerText != null)
        {
            _originalColor = timerText.color;
        }            

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

        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(_timer);
            timerText.text = seconds.ToString() + 's';
        }

        if (_timer <= 5f && !_isWarningActive)
        {
            _isWarningActive = true;
            StartCoroutine(BlinkTimerRoutine());
        }

        if (_timer < 0f)
        {
            _timer = 0f;
            StopScore();
            int finalScore = GetFinalScore();
            MinigameFlowController.Instance.EndGame(finalScore, EndType.Timeout);
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



    private IEnumerator BlinkTimerRoutine()
    {
        while (_timer > 0 && !_isPaused)
        {
            timerText.color = warningcolor;
            yield return new WaitForSeconds(blinkInterval);

            timerText.color = _originalColor;
            yield return new WaitForSeconds(blinkInterval);
        }

        timerText.color = _originalColor;
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
