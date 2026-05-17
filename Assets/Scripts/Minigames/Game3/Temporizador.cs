using UnityEngine;
using TMPro; // Para el texto

public class Temporizador : MonoBehaviour
{
    public float tiempoRestante = 20f; // Los segundos que tú quieras
    public TextMeshProUGUI textoCronometro;
    private bool cuentaActiva = true;

    void Update()
    {
        if (cuentaActiva)
        {
            if (tiempoRestante > 0)
            {
                tiempoRestante -= Time.deltaTime;
                ActualizarInterfaz();
            }
            else
            {
                tiempoRestante = 0;
                cuentaActiva = false;
                FinalizarPorTiempo();
            }
        }
    }

    void ActualizarInterfaz()
    {
        // Esto muestra solo los números enteros (sin decimales)
        if (textoCronometro != null)
            textoCronometro.text = Mathf.CeilToInt(tiempoRestante).ToString() + "s";
    }

    void FinalizarPorTiempo()
    {
        // Buscamos tu generador y forzamos el fin del juego
        GeneradorMensajes generador = Object.FindFirstObjectByType<GeneradorMensajes>();
        if (generador != null)
        {
            // Llamamos a la función de finalizar
            generador.FinalizarJuego(true); 
        }
    }
}