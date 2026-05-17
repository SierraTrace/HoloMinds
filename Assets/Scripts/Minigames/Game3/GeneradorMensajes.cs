using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Necesario si quieres forzar salto de escena

public class GeneradorMensajes : MonoBehaviour
{

    //
    public MinigameEnd minigameEndScript; // Referencia al script de fin de minijuego
    //

    [Header("Mensajes del Ex")]
    public GameObject[] prefabsEx;

    [Header("Mensajes de Familia")]
    public GameObject[] prefabsFamilia;
    
    public Transform puntoDeSpawn;

   [Header("Ritmo de Mensajes")]
    public float tiempoEntreMensajesInicial = 0.5f; 
    private float tiempoEntreMensajesActual;
    public float tiempoMinimoEntreMensajes = 0.2f; // El tope de rapidez de spawn

    [Header("Sistema de Puntuación")]
    public int puntuacionActual = 30;
    public int limitePuntosVictoria = 500; // Puntos para ganar
    public int mensajesRojosEscapados = 0;
    public int limiteErrores = 100;         // Máximo de fallos permitidos
    
    [Header("Conexión con GameManager")]
    public int indiceDelMinijuego = 2;    // Índice 2 corresponde al Minijuego 3 en el array
    public GameObject objetoBotonFinalizar;

     [Header ("Dificultad")]
    public float velocidadInicial = 2f;
    public float velocidadActual;
    public float incrementoVelocidad = 0.5f;
    public float velocidadMaxima = 15f;

    [Header("Efecto de pantalla")]
    public GameObject FlashRojo;

    [Header("Configuración para Ex audio")]
    public AudioClip error; 

    private bool juegoTerminado = false;
    private CameraShaker shaker;

    void Start()
    {
        // Al empezar, activamos la rutina de spam
        StartCoroutine(SpamMensajes());
        velocidadActual = velocidadInicial;
        //Buscamos el script en la cámara al empezar
        shaker = Camera.main.GetComponent<CameraShaker>();

        if (BarraPuntuacion.Instance != null)
        {
            BarraPuntuacion.Instance.SetMaxValue(limitePuntosVictoria);
            BarraPuntuacion.Instance.SetValue(30); // Como empieza
        }
    }
    void Update()
    {
        if (velocidadActual < velocidadMaxima)
        {
            velocidadActual += incrementoVelocidad * Time.deltaTime;
        }

        // Reducción del tiempo de espera (Cada vez salen más mensajes)
        // Hacemos que el tiempo de spawn baje proporcionalmente a la velocidad
        float progreso = (velocidadActual - velocidadInicial) / (velocidadMaxima - velocidadInicial);
        tiempoEntreMensajesActual = Mathf.Lerp(tiempoEntreMensajesInicial, tiempoMinimoEntreMensajes, progreso);
    }

    IEnumerator SpamMensajes()
    {
        while (!juegoTerminado)
        {
            yield return new WaitForSeconds(tiempoEntreMensajesActual);
            
            
            // 50% de probabilidad de que sea Ex o Familia
            bool esEx = Random.value > 0.5f;
            GameObject prefabAElegir;
            
            if (esEx)
            {
                // Elige uno al azar de la lista del Ex
                prefabAElegir = prefabsEx[Random.Range(0, prefabsEx.Length)];
            }
            else
            {
                // Elige uno al azar de la lista de Familia (Madre o Padre)
                prefabAElegir = prefabsFamilia[Random.Range(0, prefabsFamilia.Length)];
            }
            
            GameObject nuevoMensaje = Instantiate(prefabAElegir, GameObject.Find("Canvas").transform);
            nuevoMensaje.GetComponent<RectTransform>().anchoredPosition = puntoDeSpawn.GetComponent<RectTransform>().anchoredPosition;
            
            // Si el elegido pertenece a la lista de Ex, le ponemos el tag rojo
            if (esEx)
            {
                nuevoMensaje.tag = "MensajeRojo";
            }
            
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

        puntuacionActual += 10;
        Debug.Log("Puntos: " + puntuacionActual);

        if (BarraPuntuacion.Instance != null)
        {
            BarraPuntuacion.Instance.SetValue(puntuacionActual);
        }
        Debug.Log("Puntos: " + puntuacionActual);

        // Comprobamos la victoria
        /*if (puntuacionActual >= limitePuntosVictoria)
        {
            FinalizarJuego(true);
        }*/
    }

    public void RegistrarError()
    {
        if (juegoTerminado) return;

        mensajesRojosEscapados++;

        if(FlashRojo != null)
        {
            StartCoroutine(EfectoPantallaRoja());
        }

        if (error != null)
        {
            AudioSource.PlayClipAtPoint(error, Camera.main.transform.position);
        }

        //A TEMBLAR
        //(Duración: 0.2 segunfos, Fuerza: 0.3)
        if (shaker != null)
        {
            shaker.Shake(0.2f, 0.3f);
        }
        
        Debug.Log("Errores: " + mensajesRojosEscapados);

        puntuacionActual = Mathf.Max(0, puntuacionActual - 10); 
        if (BarraPuntuacion.Instance != null) BarraPuntuacion.Instance.SetValue(puntuacionActual);

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
    IEnumerator EfectoPantallaRoja()
    {
        FlashRojo.SetActive(true); // Encendemos el panel rojo
        yield return new WaitForSeconds(0.2f); // Lo dejamos encendido un instante
        FlashRojo.SetActive(false); // Lo apagamos
    }

}