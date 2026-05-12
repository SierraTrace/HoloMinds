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
            //En lugar de usar 'velocidadCaida' (que es fija),
            //le preguntamos al generador cuál es la velocidad global AHORA.
            float vGlobal = (generadorPrincipal != null) ? generadorPrincipal.velocidadActual : 2f;

            //Aplicamos el movimiento usando esa velocidad variable.
            //He multiplicado por 150f porque la velocidad del generador suele ser un número bajo (2, 3, 4...)
            //y para mover píxeles en el Canvas necesitamos números más grandes.
            transform.Translate(Vector3.down * (vGlobal * 150f) * Time.deltaTime);
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
    }
}