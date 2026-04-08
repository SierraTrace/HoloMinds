using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    // Usar este método para cargar la siguiente escena en el orden de construcción,
    // es automático y no requiere que sepas el nombre o índice de la escena siguiente.
    public static void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public static void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public static void ReloadCurrentScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public static void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
