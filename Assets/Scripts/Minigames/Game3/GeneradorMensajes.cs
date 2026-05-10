using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Necesario si quieres forzar salto de escena

public class GeneradorMensajes : MonoBehaviour
{
    public GameObject prefabExNew;
    public GameObject prefabFamilia;
    public Transform puntoDeSpawn;
    public float tiempoEntreMensajes = 2f;
    
    [Header("Sistema de Puntuación")]
    public int puntuacionActual = 0;
    public int limitePuntosVictoria = 10; // Puntos para ganar
    public int mensajesRojosEscapados = 0;
    public int limiteErrores = 3;         // Máximo de fallos permitidos
    
    [Header("Conexión con GameManager")]
    public int indiceDelMinijuego = 2;    // Índice 2 corresponde al Minijuego 3 en el array
    public GameObject objetoBotonFinalizar;

    private bool juegoTerminado = false;

    void Start()
    {
        // Al empezar, activamos la rutina de spam
        StartCoroutine(SpamMensajes());
    }

    IEnumerator SpamMensajes()
    {
        while (!juegoTerminado)
        {
            yield return new WaitForSeconds(tiempoEntreMensajes);
            
            
            GameObject prefabAElegir = (Random.value > 0.5f) ? prefabExNew : prefabFamilia;
            
            
            GameObject nuevoMensaje = Instantiate(prefabAElegir, GameObject.Find("Canvas").transform);
            
          
            nuevoMensaje.GetComponent<RectTransform>().anchoredPosition = puntoDeSpawn.GetComponent<RectTransform>().anchoredPosition;
            
           
            if (prefabAElegir == prefabExNew)
            {
                nuevoMensaje.tag = "MensajeRojo";
            }
            
            // Conectamos este generador con el script de swipear del mensaje
            Swipear scriptSwipe = nuevoMensaje.GetComponent<Swipear>();
            if (scriptSwipe != null)
            {
                scriptSwipe.generadorPrincipal = this;
            }
        }
    }

    // NUEVO: Método para sumar puntos al acertar
    public void SumarPunto()
    {
        if (juegoTerminado) return;

        puntuacionActual++;
        Debug.Log("Puntos: " + puntuacionActual);

        // Comprobamos la victoria
        if (puntuacionActual >= limitePuntosVictoria)
        {
            FinalizarJuego(true);
        }
    }

    public void RegistrarError()
    {
        if (juegoTerminado) return;

        mensajesRojosEscapados++;
        Debug.Log("Errores: " + mensajesRojosEscapados);

        // Comprobamos la derrota
        if(mensajesRojosEscapados >= limiteErrores)
        {
            FinalizarJuego(false);
        }
    }

    public void FinalizarJuego(bool esVictoria)
    {
        if (juegoTerminado) return;
        juegoTerminado = true;
        StopAllCoroutines();

        StartCoroutine(SecuenciaSalidaAutomatica());
    }

    private IEnumerator SecuenciaSalidaAutomatica()
    {
        yield return new WaitForSeconds(1f); // pausa

        MinigameEnd endScript = Object.FindFirstObjectByType<MinigameEnd>();

        if (endScript != null)
        {
            endScript.FinishMinigameWithScore(puntuacionActual);
        }
        else
        {
            Debug.LogError("MinigameEnd no encontrado");
        }
    }

    // Versión anterior
    /*public void FinalizarJuego(bool esVictoria)
    {
        juegoTerminado = true;
        StopAllCoroutines(); 

       
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(indiceDelMinijuego, puntuacionActual);
            Debug.Log($"Guardando {puntuacionActual} puntos en el nivel {indiceDelMinijuego} del GameManager.");
        }
        else
        {
            Debug.LogWarning("No se ha encontrado el GameManager.Instance en la escena.");
        }

        // Acciones visuales del fin de juego
        if (objetoBotonFinalizar != null)
        {
            objetoBotonFinalizar.SetActive(true);
        }

       
        if (esVictoria) SceneManager.LoadScene("Game4_AR"); 
        
    }*/
}