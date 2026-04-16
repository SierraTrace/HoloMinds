using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.Instance.ResetScores(); // Reiniciar las puntuaciones al iniciar un nuevo juego
        SceneManager.LoadScene("Game1_2D");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
