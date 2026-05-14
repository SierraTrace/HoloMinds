using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections; 

public class ZonaBasura : MonoBehaviour, IDropHandler
{
    [Header("Configuración para Ex")]
    public GameObject EfectoTranquilidad; 
    public AudioClip sonidoTranquilidad; 
    public Transform contenedorEfectos; 

    [Header("Efecto de pantalla")]
    public GameObject FlashVerde;

    [Header("Efecto de pantalla")]
    public GameObject FlashRojo;

   private CameraShaker shaker;

   [Header("Configuración para Familia")]
    public AudioClip error; 

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        GameObject mensaje = eventData.pointerDrag;

        if(mensaje.CompareTag("MensajeRojo"))
        {
            //    
            GeneradorMensajes gen = Object.FindFirstObjectByType<GeneradorMensajes>();
            if (gen != null)
            {
                gen.SumarPunto();
            }
            //

            EjecutarFeedBack(mensaje);
            Debug.Log("Animaciones hechas");
        }
        else
        {
            ColocarEnBasura(mensaje);
        }
    }

    private void EjecutarFeedBack(GameObject mensaje)
    {

        //PUNTUACION SUPER IMPORTANTE NO BORRAR
        GeneradorMensajes gen = Object.FindFirstObjectByType<GeneradorMensajes>();
        if (gen != null) gen.SumarPunto();
        


        if (EfectoTranquilidad != null)
        {
            // 1. Instanciamos la estrella DENTRO del contenedor visible, no en 'this.transform'
            GameObject efecto = Instantiate(EfectoTranquilidad, contenedorEfectos);
            
            // 2. La movemos a la posición de la papelera para que parezca que sale de ahí
            efecto.transform.position = this.transform.position;
            
            // 3. Forzamos la escala para que no se vea rara
            efecto.transform.localScale = Vector3.one;

            // Destruimos el efecto después de 1 segundo
            Destroy(efecto, 0.5f);
        }

        // Sonido
        if (sonidoTranquilidad != null)
        {
            AudioSource.PlayClipAtPoint(sonidoTranquilidad, Camera.main.transform.position);
        }

        // Animación de desaparecer el mensaje
        StartCoroutine(AnimacionDesaparecer(mensaje));

        if (FlashVerde != null)
        {
            StartCoroutine(EfectoPantallaVerde());
        }
    }

    private void ColocarEnBasura(GameObject mensaje)
    {
        if (mensaje.CompareTag("MensajeFamilia") || !mensaje.CompareTag("MensajeRojo"))
        {
            // Sonido
            if (error!= null)
            {
            StartCoroutine(AnimacionDesaparecer(mensaje));

            AudioSource.PlayClipAtPoint(error, Camera.main.transform.position);
            }
            if (FlashRojo != null) StartCoroutine(EfectoPantallaRoja());
            if (shaker != null) shaker.Shake(0.2f, 0.4f);

            // Animación de desaparecer el mensaje
            
            //Registramos el error en el generador para restar autoestima
            GeneradorMensajes gen = Object.FindFirstObjectByType<GeneradorMensajes>();

            if (gen != null) gen.RegistrarError();

            Debug.Log("¡Error! Has tirado a la familia.");
        }
        else
        {
            Debug.Log("¡Bien! Ex eliminado.");

            // Si queremos que el acierto de la basura sume puntos
            GeneradorMensajes gen = Object.FindFirstObjectByType<GeneradorMensajes>();
            if (gen != null) gen.SumarPunto();
        }

        // Simplemente destruimos el mensaje si no es el rojo
        //Destroy(mensaje);
    }

    private IEnumerator AnimacionDesaparecer(GameObject obj)
    {
        Vector3 escalaInicial = obj.transform.localScale; 
        float duracion = 0.25f; 
        float tiempo = 0f; 

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime; 
            obj.transform.localScale = Vector3.Lerp(escalaInicial, Vector3.zero, tiempo / duracion);
            yield return null; 
        }
        
        Destroy(obj); 
    }
    IEnumerator EfectoPantallaVerde()
    {
    FlashVerde.SetActive(true); // Encendemos el panel verde
    yield return new WaitForSeconds(0.2f); // Lo dejamos encendido un instante
    FlashVerde.SetActive(false); // Lo apagamos
    }
    IEnumerator EfectoPantallaRoja()
    {
    FlashRojo.SetActive(true); // Encendemos el panel rojo
    yield return new WaitForSeconds(0.2f); // Lo dejamos encendido un instante
    FlashRojo.SetActive(false); // Lo apagamos
    }

    
}