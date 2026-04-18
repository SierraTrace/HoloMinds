using UnityEngine;
using TMPro;

public class ControladorTiempoAR : MonoBehaviour
{
    public float tiempoRestante = 30f;
    public TextMeshProUGUI textoTemporizador; 
               
    
    private bool juegoTerminado = false;

 

    void Update()
    {
        if (!juegoTerminado)
        {
            if (tiempoRestante > 0)
            {
                tiempoRestante -= Time.deltaTime;
                ActualizarReloj(tiempoRestante);
            }
            else
            {
                TerminarMicrojuego();
            }
        }
    }

    void ActualizarReloj(float tiempo)
    {
        textoTemporizador.text = Mathf.CeilToInt(tiempo).ToString() + "s";
    }

    public void TerminarMicrojuego()
    {
        juegoTerminado = true;
        tiempoRestante = 0;

        textoTemporizador.gameObject.SetActive(false);

        // 2. Buscamos los scripts necesarios
        InteraccionAR scriptInteraccion = FindFirstObjectByType<InteraccionAR>();
        MinigameEnd scriptFinalCompas = FindFirstObjectByType<MinigameEnd>();

        // 3. PASAMOS LOS PUNTOS AL SCRIPT DE DANI
        if (scriptInteraccion != null && scriptFinalCompas != null)
        {
            scriptFinalCompas.testScore = scriptInteraccion.puntosLocales;
           SceneLoader.LoadNextScene();
        }
    }
}