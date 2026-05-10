using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections; //Para utilziar Corrutinas, animacioens de tiempo

public class ZonaBasura : MonoBehaviour, IDropHandler
{
    //Cremos una etiqueta visual en inspector de unity para organizar las variables
    [Header("Configuración para Ex")]
    public GameObject prefabEfectoTranquilidad; //Efecto particulas
    public AudioClip sonidoTranquilidad; //Aqui ponemos el audio de tranquilidad
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        GameObject mensaje = eventData.pointerDrag;
        Swipear scriptSwipear = mensaje.GetComponent<Swipear>();

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
        //Primero instanciamos los efectos de las particulas 
        if (prefabEfectoTranquilidad != null)
        {
            Vector3 posicionFija = new Vector3(0, 0, 0); 
            GameObject efecto = Instantiate(prefabEfectoTranquilidad, posicionFija, Quaternion.identity);
            //Distruimos el efecto después de 1 segundo para no saturar la memoria
            Destroy(efecto, 1f);
        }
        //Sonido, reproducimos el sonido en la posición de la camara
        if (sonidoTranquilidad != null)
        {
            AudioSource.PlayClipAtPoint(sonidoTranquilidad, Camera.main.transform.position);
        }
        //Iniciamos la animacion de desaparición mediante una Corrutina
        StartCoroutine(AnimacionDesaparecer(mensaje));
    }
    private void ColocarEnBasura(GameObject mensaje)
    {
        //Convertimos la papelera en el "padre" del mensaje para que se mueva con ella
        mensaje.transform.SetParent(this.transform);

        //Obtenemos el tamaño del area de la papelera
        RectTransform trashRect = GetComponent<RectTransform>();

        // Calculamos una posición aleatoria (X e Y) dentro de los límites de la papelera
        // Restamos 50 para que no aparezcan justo en el borde y se salgan de la caja
        float x = Random.Range(-(trashRect.rect.width/2)+50, (trashRect.rect.width/2)-50);
        float y = Random.Range(-(trashRect.rect.height/2)+50, (trashRect.rect.height/2)-50);

        //Aplicamos la posicion calculada
        mensaje.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
    }

    // Las Corrutinas permiten hacer acciones "durante" un tiempo (ej. animar de poco a poco)
    private IEnumerator AnimacionDesaparecer(GameObject obj)
    {
        Vector3 escalaInicial = obj.transform.localScale; // Guardamos el tamaño original
        float duracion = 0.25f; // Duración muy corta para que la desaparición sea instantánea y "snappy"
        float tiempo = 0f; // Contador de tiempo

        // Bucle que corre frame a frame hasta alcanzar la duración deseada
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime; // Sumamos el tiempo transcurrido desde el último frame
            
            // Lerp (Linear Interpolation) va cambiando el tamaño de escalaInicial a cero suavemente
            obj.transform.localScale = Vector3.Lerp(escalaInicial, Vector3.zero, tiempo / duracion);
            
            // Esperamos al siguiente frame para seguir ejecutando el bucle
            yield return null; 
        }
        
        // Al terminar el bucle, destruimos el objeto de la escena definitivamente
        Destroy(obj); 
    }
}