using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.Instance.ResetScores(); // Reiniciar las puntuaciones al iniciar un nuevo juego
        SceneLoader.LoadNextScene();        // Cargar la siguiente escena en el orden de construcción
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
