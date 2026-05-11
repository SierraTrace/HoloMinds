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

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        GameObject mensaje = eventData.pointerDrag;

        if(mensaje.CompareTag("MensajeRojo"))
        {
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
        // Simplemente destruimos el mensaje si no es el rojo
        Destroy(mensaje);
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
    
    
}