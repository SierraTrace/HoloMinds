using UnityEngine;

public class MinigameEnd : MonoBehaviour
{
    public int minigameIndex;   // ï¿½ndice del minijuego actual, asignado en el Inspector
    public int testScore = 100; // TODO Puntuaciï¿½n de prueba para simular el resultado del minijuego

    public void FinishMinigame()  // Método para finalizar el minijuego sin una puntuación real (usado para pruebas)
    {

        Debug.Log($"Minigame {minigameIndex} finished with score: {testScore}");

        GameManager.Instance.AddScore(minigameIndex, testScore);
        SceneLoader.LoadNextScene();
    }

    public void FinishMinigameWithScore(int score) // Método para finalizar el minijuego con una puntuación real (usado en integración final)
    {
        Debug.Log($"Minigame {minigameIndex} finished with score: {score}");
        
        GameManager.Instance.AddScore(minigameIndex, score);
        SceneLoader.LoadNextScene();
    }

}
