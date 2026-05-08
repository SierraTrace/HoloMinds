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

        for(int i = 0; i < scores.Length; i++)
        {
            yield return new WaitForSeconds(delayBetweenScores);

            scoresListText.text += $"Juego {i + 1}: {scores[i]}\n";

            // TODO: Llamada a sonido de puntuación aquí
        }

        yield return new WaitForSeconds(1f);

        float currentDisplayScore = 0;
        while (currentDisplayScore < GameManager.Instance.totalScore)
        {
            currentDisplayScore += Time.deltaTime * scoreCountSpeed;
            totalScoreText.text = "TOTAL: " + Mathf.Round(currentDisplayScore);
            yield return null;
        }

        // totalScoreText.text = "Puntuación Total: " + GameManager.Instance.totalScore;

        yield return new WaitForSeconds(0.5f);

        mainMenuButton.SetActive(true);
    }


    public void BackToMainMenu()
    {
        SceneLoader.LoadSceneByName("MainMenu");
    }
}
