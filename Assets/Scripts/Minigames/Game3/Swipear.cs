using UnityEngine;
using UnityEngine.EventSystems;

public class Swipear : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // AUMENTADO: Ahora los mensajes caen un poco más rápido
    public float velocidadCaida = 450f; 
    private bool estaArrastrado = false;
    public GeneradorMensajes generadorPrincipal;
    private CanvasGroup canvasGroup;
    private Transform parentOriginal;
    public bool estaBasura = false;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
       
        if (!estaArrastrado && !estaBasura)
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
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentOriginal = transform.parent;
        canvasGroup.blocksRaycasts = false;
        estaArrastrado = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        estaArrastrado = true;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        estaArrastrado = false;
        canvasGroup.blocksRaycasts = true;

        if(transform.parent != parentOriginal)
        {
            return;
        }

        //>Dani     Si hacemos esto, el mensaje se destruye al soltarlo fuera de la papelera, lo que no es deseable.
        //          Solo queremos destruirlo si se suelta dentro de la papelera, lo cual se maneja en ZonaBasura.cs

        /*
        if (Mathf.Abs(transform.localPosition.x) > 300f)
        {
            
            if (gameObject.CompareTag("MensajeRojo") && generadorPrincipal != null)
            {
                generadorPrincipal.SumarPunto();
            }

            Destroy(gameObject);
        }
        */
        //<Dani
    }
}