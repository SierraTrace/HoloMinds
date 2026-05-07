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
    public BonusAnimator bonusAnimator;
    public Transform scoreTargetPosition;

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
            StartCoroutine(FinalSequenceWithBonus());
        }

        // Calcular distancia
        _currentDistance += gameSpeed * Time.deltaTime;

        UpdateScoreUI();
    }



    private IEnumerator FinalSequenceWithBonus()
    {
        
        if (MinigameFlowController.Instance != null)
        {
            StopScore();
            prepareVisualStop();
        }

        yield return new WaitForSeconds(0.5f);

        if (bonusAnimator != null)
        {
            Vector3 centerScreen = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            bonusAnimator.StartAnimation(centerScreen, scoreTargetPosition);
        }

        yield return new WaitForSeconds(0.9f);

        _currentDistance += 100;
        UpdateScoreUI();

        yield return new WaitForSeconds(0.5f);

        int finalScore = GetFinalScore();
        MinigameEnd end = FindObjectOfType<MinigameEnd>();
        if (end != null)
        {
            end.FinishMinigameWithScore(finalScore);
        }
    }



    private IEnumerator BlinkTimerRoutine()
    {
        while (_timer > 0 && !_isPaused)
        {
            timerText.color = warningcolor;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayTimerWarning();
            }

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


    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = Mathf.FloorToInt(_currentDistance).ToString() + " m";
        }
        if (SelfEsteemBar.Instance != null)
        {
            SelfEsteemBar.Instance.SetValue(_currentDistance);
        }
    }


    private void prepareVisualStop()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBackgroundMusic();
        }

        ParallaxController parallax = FindObjectOfType<ParallaxController>();
        if (parallax != null)
        {
            parallax.globalSpeed = 0f;
        }

        foreach (var obstacle in FindObjectsOfType<ObstacleMovement>())
        {
            obstacle.enabled = false;
        }

        ObstacleSpawner spawner = FindObjectOfType<ObstacleSpawner>();
        if (spawner != null)
        {
            spawner.StopAllCoroutines();
        }

        PlayerJump player = FindObjectOfType<PlayerJump>();
        if (player != null)
        {
            player.enabled = false;
        }

    }


}
