using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Necesario si quieres forzar salto de escena

public class GeneradorMensajes : MonoBehaviour
{

    //>Dani
    public MinigameEnd minigameEndScript; // Referencia al script de fin de minijuego
    //>Dani

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

     [Header ("Dificultad")]
    public float velocidadInicial = 2f;
    public float velocidadActual;
    public float incrementoVelocidad = 0.1f;
    public float velocidadMaxima = 10f;

    private bool juegoTerminado = false;
    private CameraShaker shaker;

    void Start()
    {
        // Al empezar, activamos la rutina de spam
        StartCoroutine(SpamMensajes());
        velocidadActual = velocidadInicial;
        //Buscamos el script en la cámara al empezar
        shaker = Camera.main.GetComponent<CameraShaker>();
    }
    void Update()
    {
        if (velocidadActual < velocidadMaxima)
        {
            velocidadActual += incrementoVelocidad * Time.deltaTime;
        }
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

    //Método para sumar puntos al acertar
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

        //A TEMBLAR
        //(Duración: 0.2 segunfos, Fuerza: 0.3)
        if (shaker != null)
        {
            shaker.Shake(0.2f, 0.3f);
        }
        
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


        //Enviamos la puntuación al script de fin de minijuego. Lo he asignado directamente en el inspector para evitar problemas de búsqueda.
        if (minigameEndScript != null)
        {
            Debug.Log("Enviando puntuación: " + puntuacionActual);
            minigameEndScript.FinishMinigameWithScore(puntuacionActual);
        }
        else
        {
            Debug.LogError("GeneradorMensajes:MinigameEnd no asignado en el inspector");
        }
        
    }

}