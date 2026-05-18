using UnityEngine;
using TMPro;
using System.Collections;

public class FinalScoreUI : MonoBehaviour
{
    [Header("Referencias de UI")]
    public TextMeshProUGUI scoresListText;
    public TextMeshProUGUI totalScoreText;
    public GameObject mainMenuButton;

    [Header("Configuración")]
    public float delayBetweenScores = 0.8f;
    public float scoreCountSpeed = 100f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip scoreAppearClip;
    public AudioClip totalCountingClip;
    public float totalSoundInterval = 0.05f;

    private float _totalSoundTimer = 0f;

    void Start()
    {
        scoresListText.text = "";
        totalScoreText.text = "";
        mainMenuButton.SetActive(false);

        StartCoroutine(ShowScoresSequence());
    }

    private IEnumerator ShowScoresSequence()
    {
        int[] scores = GameManager.Instance.gameScores;

        // Mostrar puntuaciones individuales
        for (int i = 0; i < scores.Length; i++)
        {
            yield return new WaitForSeconds(delayBetweenScores);

            scoresListText.text += $"Juego {i + 1}: {scores[i]}\n";

            
            if (audioSource != null && scoreAppearClip != null)
                audioSource.PlayOneShot(scoreAppearClip);
        }

        yield return new WaitForSeconds(1f);

        // Conteo animado del total
        float currentDisplayScore = 0;
        int finalScore = GameManager.Instance.totalScore;

        while (currentDisplayScore < finalScore)
        {
            currentDisplayScore += Time.deltaTime * scoreCountSpeed;
            totalScoreText.text = "TOTAL: " + Mathf.Round(currentDisplayScore);

            
            _totalSoundTimer += Time.deltaTime;
            if (_totalSoundTimer >= totalSoundInterval)
            {
                if (audioSource != null && totalCountingClip != null)
                    audioSource.PlayOneShot(totalCountingClip);

                _totalSoundTimer = 0f;
            }

            yield return null;
        }

        totalScoreText.text = "TOTAL: " + finalScore;

        yield return new WaitForSeconds(0.5f);

        mainMenuButton.SetActive(true);
    }

    public void BackToMainMenu()
    {
        SceneLoader.LoadSceneByName("MainMenu");
    }
}
