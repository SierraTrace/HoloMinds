using UnityEngine;
using System.Collections;

public class GeneradorMensajes : MonoBehaviour
{
    public GameObject prefabEx;
    public GameObject prefabFamilia;
    public Transform puntoDeSpawn;
    public float tiempoEntreMensajes = 2f;
    public int mensajesRojosEscapados = 0;
    public int limiteErrores = 5;
    public GameObject objetoBotonFinalizar;

    void Start()
    {
        // Al empezar, activamos la rutina de spam
        StartCoroutine(SpamMensajes());
    }

    IEnumerator SpamMensajes()
    {
        while (true)
        {
            yield return new WaitForSeconds(tiempoEntreMensajes);
            
            // Elegimos aleatoriamente entre los dos
            GameObject prefabAElegir = (Random.value > 0.5f) ? prefabEx : prefabFamilia;
            
            // Lo creamos y lo metemos en el Canvas automáticamente
            GameObject nuevoMensaje = Instantiate(prefabAElegir, GameObject.Find("Canvas").transform);
            
            // Lo posicionamos donde esté tu PuntoSpawn
            nuevoMensaje.GetComponent<RectTransform>().anchoredPosition = puntoDeSpawn.GetComponent<RectTransform>().anchoredPosition;
            
            // Si es del Ex, le ponemos el Tag para detectarlo al caer
            if (prefabAElegir == prefabEx)
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

    public void RegistrarError()
    {
        mensajesRojosEscapados++;
        Debug.Log("Errores: " + mensajesRojosEscapados);

        if(mensajesRojosEscapados >= limiteErrores)
        {
            FinalizarJuego();
        }
    }

    public void FinalizarJuego()
    {
        StopAllCoroutines();
        if (objetoBotonFinalizar != null)
        {
            objetoBotonFinalizar.SetActive(true);
        }
    }
}