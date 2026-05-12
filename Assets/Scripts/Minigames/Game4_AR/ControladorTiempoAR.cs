using UnityEngine;
using TMPro;
using System.Collections;

public class ControladorTiempoAR : MonoBehaviour
{
    public float tiempoRestante = 60f;
    public TextMeshProUGUI textoTemporizador;

    [Header("Aviso Final")]
    public float tiempoAviso = 10f;
    public Color colorAviso = Color.red;
    public float intervaloParpadeo = 0.5f;

    private bool juegoTerminado = false;
    private bool avisoActivado = false;
    private Color colorOriginal;

    void Start()
    {
        if (textoTemporizador != null)
            colorOriginal = textoTemporizador.color;
    }

    void Update()
    {
        if (juegoTerminado) return;

        if (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;
            ActualizarReloj(tiempoRestante);

            if (tiempoRestante <= tiempoAviso && !avisoActivado)
            {
                avisoActivado = true;
                StartCoroutine(ParpadeaTimer());
            }
        }
        else
        {
            TerminarMicrojuego();
        }
    }

    void ActualizarReloj(float tiempo)
    {
        textoTemporizador.text = Mathf.CeilToInt(tiempo).ToString() + "s";
    }

    IEnumerator ParpadeaTimer()
    {
        while (tiempoRestante > 0 && !juegoTerminado)
        {
            textoTemporizador.color = colorAviso;
            yield return new WaitForSeconds(intervaloParpadeo);
            textoTemporizador.color = colorOriginal;
            yield return new WaitForSeconds(intervaloParpadeo);
        }

        if (textoTemporizador != null)
            textoTemporizador.color = colorOriginal;
    }

    public void TerminarMicrojuego()
    {
        juegoTerminado = true;
        tiempoRestante = 0;

        StopAllCoroutines();
        textoTemporizador.gameObject.SetActive(false);

        InteraccionAR scriptInteraccion = FindFirstObjectByType<InteraccionAR>();
        MinigameEnd scriptFinalCompas = FindFirstObjectByType<MinigameEnd>();

        if (scriptInteraccion != null && scriptFinalCompas != null)
        {
            scriptFinalCompas.FinishMinigameWithScore(scriptInteraccion.puntosLocales);
        }
    }
}
