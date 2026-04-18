using UnityEngine;
using UnityEngine.InputSystem; 
using UnityEngine.UI; 
public class InteraccionAR : MonoBehaviour
{
    [HideInInspector] public int puntosLocales = 0; 

    [Header("Interfaz")]
    public Slider barraPuntos; 
    public int puntosMaximos = 10; 

    [Header("Efectos Visuales")]
    public GameObject prefabParticulasBuenas;
    public GameObject prefabParticulasMalas;

    void Start()
    {
        // Configuramos la barra al empezar el juego
        if (barraPuntos != null)
        {
            barraPuntos.maxValue = puntosMaximos;
            barraPuntos.value = puntosLocales;
        }
    }

    void Update()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            Vector2 posicionPantalla = Pointer.current.position.ReadValue();
            Ray rayo = Camera.main.ScreenPointToRay(posicionPantalla);
            RaycastHit[] impactos = Physics.RaycastAll(rayo);

            foreach (RaycastHit hit in impactos)
            {
                ObjetoAR objetoTocado = hit.transform.GetComponent<ObjetoAR>();

                if (objetoTocado != null)
                {
                    // Si tocamos el bueno (Suma)
                    if (objetoTocado.debeElimibarse)
                    {
                        puntosLocales += objetoTocado.valorPuntos;
                        if (prefabParticulasBuenas != null) 
                            Instantiate(prefabParticulasBuenas, hit.transform.position, Quaternion.identity);
                    }
                    // Si tocamos el malo (Resta)
                    else
                    {
                        puntosLocales -= objetoTocado.valorPuntos;
                        // Evitamos que los puntos bajen de cero
                        if (puntosLocales < 0) puntosLocales = 0; 
                        
                        if (prefabParticulasMalas != null) 
                            Instantiate(prefabParticulasMalas, hit.transform.position, Quaternion.identity);
                    }

                    // ACTUALIZAMOS LA BARRA VISUALMENTE
                    if (barraPuntos != null)
                    {
                        barraPuntos.value = puntosLocales;
                    }

                    Destroy(hit.transform.gameObject);
                    break; 
                }
            }
        }
    }
}