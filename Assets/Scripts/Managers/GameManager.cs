using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int totalScore;                      // Puntuación total acumulada por el jugador
    public int[] levelScores = new int[5];      // Guardaremos las puntuaciones de cada nivel

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int levelIndex, int score)
    {
        if (levelIndex >= 0 && levelIndex < levelScores.Length)
        {
            levelScores[levelIndex] = score;
            RecalculateTotal();
        }
    }

    void RecalculateTotal()
    {
        totalScore = 0;
        foreach (int score in levelScores)
        {
            totalScore += score;
        }
    }
}