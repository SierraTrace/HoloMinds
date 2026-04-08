using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int totalScore = 0;                  // Puntuación total acumulada por el jugador
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
}