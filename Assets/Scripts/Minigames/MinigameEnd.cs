using UnityEngine;

public class MinigameEnd : MonoBehaviour
{
    public int minigameIndex;   // Índice del minijuego actual, asignado en el Inspector
    public int testScore = 100; // TODO Puntuación de prueba para simular el resultado del minijuego

    public void FinishMinigame()
    {
        // TODO Aquí se debería pasar la puntuación real del minijuego en lugar de usar testScore
        // Retirar el Debug.Log una vez que se integre con la puntuación real
        Debug.Log($"Minigame {minigameIndex} finished with score: {testScore}");
        
        GameManager.Instance.AddScore(minigameIndex, testScore);
        SceneLoader.LoadNextScene();
    }
}
