using UnityEngine;
using TMPro;

public class ControladorTiempoAR : MonoBehaviour
{
    public float tiempoRestante = 30f;
    public TextMeshProUGUI textoTemporizador; 
    public GameObject botonFinal;            
    
    private InteraccionAR scriptInteraccion;
    private MinigameEnd scriptFinalCompas;
    private bool juegoTerminado = false;

    void Start()
    {
        if (botonFinal != null) botonFinal.SetActive(false);
        
        scriptInteraccion = FindFirstObjectByType<InteraccionAR>();
        scriptFinalCompas = FindFirstObjectByType<MinigameEnd>();
    }

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

    void TerminarMicrojuego()
    {
        juegoTerminado = true;
        tiempoRestante = 0;
        ActualizarReloj(0);

        if (botonFinal != null) botonFinal.SetActive(true);
        textoTemporizador.gameObject.SetActive(false);

        // PASAMOS LOS PUNTOS AL SCRIPT DE DANI
        if (scriptInteraccion != null && scriptFinalCompas != null)
        {
            scriptFinalCompas.testScore = scriptInteraccion.puntosLocales;
            Debug.Log("Puntos enviados al script final: " + scriptFinalCompas.testScore);
        }
    }

    // Este método lo llamarás desde el evento OnClick del botón
    public void BotonPulsado()
    {
        if (scriptFinalCompas != null)
        {
            scriptFinalCompas.FinishMinigame(); // Usa el método de Dani
        }
    }
}