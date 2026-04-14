using UnityEngine;
using UnityEngine.InputSystem; // Usamos el Input System que sí te funcionaba

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
        // 1. Movimiento
        transform.position += direccion * velocidad * Time.deltaTime;

        // 2. DETECCIÓN DE CLIC (La que te funcionaba)
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            DetectarToque();
        }

        // 3. Autodestrucción por distancia
        if (Vector3.Distance(Vector3.zero, transform.position) > 10f)
        {
            if (esDelEx && GameManagerAutoestima.instance != null)
                GameManagerAutoestima.instance.ModificarAutoestima(-5);
            Destroy(gameObject);
        }
    }

    void DetectarToque()
    {
        // Convertimos la posición del ratón a coordenadas del mundo 2D
        Vector2 posicionPantalla = Pointer.current.position.ReadValue();
        Vector2 posicionMundo = Camera.main.ScreenToWorldPoint(posicionPantalla);

        // LANZAMOS UN "PINCHAZO" VIRTUAL
        // Esto comprueba si hay un collider justo donde hemos pinchado
        RaycastHit2D hit = Physics2D.Raycast(posicionMundo, Vector2.zero);

        // LA LLAVE DE SEGURIDAD:
        // Solo si el pinchazo toca un objeto Y ese objeto es precisamente ESTE (gameObject)
        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            ProcesarAcierto();
        }
    }

    void ProcesarAcierto()
    {
        if (GameManagerAutoestima.instance == null || !GameManagerAutoestima.instance.juegoActivo) return;

        if (esDelEx)
        {
            GameManagerAutoestima.instance.ModificarAutoestima(15);
            GameManagerAutoestima.instance.PlaySonido(true);
            if (purpurinaPrefab != null) Instantiate(purpurinaPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            GameManagerAutoestima.instance.ModificarAutoestima(-10);
            GameManagerAutoestima.instance.PlaySonido(false);
        }

        Destroy(gameObject);
    }
}