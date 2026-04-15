using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;    // Singleton instance

    [Header("Configuration")]
    public TextMeshProUGUI scoreText;
    public float gameSpeed = 5f;

    private float _currentDistance;
    privated bool _isPaused = false;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (_isPaused) return;

        // Distancia = Velocidad * Tiempo
        _currentDistance += gameSpeed * Time.deltaTime;

        if (scoreText != null)
        {
            scoreText.text = Mathf.FloorToInt(_currentDistance).ToString() + " m";
        }
    }

    public int GetFinalScore() => Mathf.FloorToInt(_currentDistance);

    public void StopScore() => _isPaused = true;

}
