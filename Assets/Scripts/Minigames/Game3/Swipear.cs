using UnityEngine;
using UnityEngine.EventSystems;

public class Swipear : MonoBehaviour, IDragHandler, IEndDragHandler
{
    // AUMENTADO: Ahora los mensajes caen un poco más rápido
    public float velocidadCaida = 450f; 
    private bool estaArrastrado = false;
    public GeneradorMensajes generadorPrincipal;

    void Update()
    {
       
        if (!estaArrastrado)
        {
            transform.Translate(Vector3.down * velocidadCaida * Time.deltaTime);
        }

        
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
        
        
        if (Mathf.Abs(transform.localPosition.x) > 300f)
        {
            
            if (gameObject.CompareTag("MensajeRojo") && generadorPrincipal != null)
            {
                generadorPrincipal.SumarPunto();
            }

            Destroy(gameObject);
        }
    }
}