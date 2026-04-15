using UnityEngine;

public class MinigameEnd : MonoBehaviour
{
    public int minigameIndex;   // �ndice del minijuego actual, asignado en el Inspector
    public int testScore = 100; // TODO Puntuaci�n de prueba para simular el resultado del minijuego

    public void FinishMinigame()
    {
        // TODO Aqu� se deber�a pasar la puntuaci�n real del minijuego en lugar de usar testScore
        // Retirar el Debug.Log una vez que se integre con la puntuaci�n real
        Debug.Log($"Minigame {minigameIndex} finished with score: {testScore}");

        GameManager.Instance.AddScore(minigameIndex, testScore);
        SceneLoader.LoadNextScene();
    }
}
