using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Game1_2D");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
