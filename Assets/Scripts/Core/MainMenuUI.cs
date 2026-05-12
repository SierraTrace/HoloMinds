using System.Collections;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [Header("Referencias de Audio")]
    [SerializeField] private float fadeOut = 1.5f;


    [Header("Referencias de Animación")]
    [SerializeField] private MainMenuAnimator animadorMenu;


    private bool estaCambiandoEscena = false;

    public void StartGame()
    {
        if (estaCambiandoEscena) return;
        StartCoroutine(SequenceStartGame());
    }

    private IEnumerator SequenceStartGame()
    {
        estaCambiandoEscena = true;

        if (MenuAudioManager.Instance != null)
        {
            MenuAudioManager.Instance.FadeOutMusica(fadeOut);
        }


        yield return new WaitForSeconds(fadeOut + 0.2f);

        
        GameManager.Instance.ResetScores();
        Debug.Log("Reiniciando puntuaciones...");
        SceneLoader.LoadNextScene();
        Debug.Log("Cargando siguiente escena...");
    }



    public void ExitGame()
    {
        if (estaCambiandoEscena) return;
        StartCoroutine(SequenceExitGame());
    }

    private IEnumerator SequenceExitGame()
    {
        estaCambiandoEscena = true;

        if (MenuAudioManager.Instance != null)
        {
            MenuAudioManager.Instance.FadeOutMusica(fadeOut);
        }

        if (animadorMenu != null)
        {
            animadorMenu.DesactivarBotones();
        }

        yield return new WaitForSeconds(fadeOut + 0.2f);

        Debug.Log("Saliendo del juego...");
        Application.Quit();

    }
}
