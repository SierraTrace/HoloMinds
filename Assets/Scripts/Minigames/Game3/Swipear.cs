using UnityEngine;
using UnityEngine.EventSystems;

public class Swipear : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public float velocidadCaida = 300f;
    private bool estaArrastrado = false;
    public GeneradorMensajes generadorPrincipal;

    void Update()
    {
        // Si el jugador no lo está tocando, el mensaje cae
        if (!estaArrastrado)
        {
            transform.Translate(Vector3.down * velocidadCaida * Time.deltaTime);
        }

        // Si el mensaje se sale por abajo de la pantalla
        if (transform.localPosition.y < -600f) 
        {
            // Si era un mensaje rojo y no se swipeó, cuenta como error
            if (gameObject.CompareTag("MensajeRojo") && generadorPrincipal != null)
            {
                generadorPrincipal.RegistrarError();
            }
            Destroy(gameObject);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        estaArrastrado = true;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        estaArrastrado = false;
        
        // Si lo lanzamos lejos a la izquierda o derecha, se destruye (lo salvamos)
        if (Mathf.Abs(transform.localPosition.x) > 300f)
        {
            Destroy(gameObject);
        }
    }
}