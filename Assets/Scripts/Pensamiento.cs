using UnityEngine;
using UnityEngine.InputSystem; // ¡IMPORTANTE: Añade esta línea!

public class Pensamiento : MonoBehaviour
{
    public bool esDelEx;
    private float velocidad;
    private Vector3 direccion;
    
    [Header("Efectos")]
    public GameObject purpurinaPrefab;

    public void Configurar(Vector3 dir, bool tipo, Sprite img, float vel)
    {
        direccion = dir;
        esDelEx = tipo;
        velocidad = vel;
        GetComponent<SpriteRenderer>().sprite = img;
    }

    void Update()
    {
        // 1. Movimiento constante
        transform.position += direccion * velocidad * Time.deltaTime;

        // 2. DETECCIÓN MODERNA (Input System 2026)
        // Detecta si se ha pulsado el ratón o la pantalla este fotograma
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            DetectarToque();
        }

        // 3. Destrucción por distancia
        if (Vector3.Distance(Vector3.zero, transform.position) > 10f)
        {
            if (esDelEx) GameManager.instance.ModificarAutoestima(-5);
            Destroy(gameObject);
        }
    }

    void DetectarToque()
    {
        // Convertimos la posición del puntero (ratón o dedo) al mundo 2D
        Vector2 posicionPantalla = Pointer.current.position.ReadValue();
        Vector2 posicionMundo = Camera.main.ScreenToWorldPoint(posicionPantalla);
        
        // Comprobamos si el punto tocado colisiona con este objeto
        Collider2D hit = Physics2D.OverlapPoint(posicionMundo);
        
        if (hit != null && hit.gameObject == gameObject)
        {
            ProcesarAcierto();
        }
    }

    void ProcesarAcierto()
    {
        if (!GameManager.instance.juegoActivo) return;

        if (esDelEx)
        {
            GameManager.instance.ModificarAutoestima(10);
            if(purpurinaPrefab != null) 
                Instantiate(purpurinaPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            GameManager.instance.ModificarAutoestima(-15);
        }
        Destroy(gameObject);
    }
}