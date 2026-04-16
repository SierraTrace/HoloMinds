using UnityEngine;
using UnityEngine.InputSystem; 

public class InteraccionAR : MonoBehaviour
{
    [HideInInspector] public int puntosLocales = 0; 

    [Header("Efectos Visuales")]
    public GameObject prefabParticulasBuenas;
    public GameObject prefabParticulasMalas;

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
                    // Si tocamos el bueno (Suma puntos y partículas alegres)
                    if (objetoTocado.debeElimibarse)
                    {
                        puntosLocales += objetoTocado.valorPuntos;
                        if (prefabParticulasBuenas != null) 
                        {
                            Instantiate(prefabParticulasBuenas, hit.transform.position, Quaternion.identity);
                        }
                    }
                    // Si tocamos el malo (Resta puntos y partículas de error)
                    else
                    {
                        puntosLocales -= objetoTocado.valorPuntos;
                        if (prefabParticulasMalas != null) 
                        {
                            Instantiate(prefabParticulasMalas, hit.transform.position, Quaternion.identity);
                        }
                    }

                    Destroy(hit.transform.gameObject);
                    break; 
                }
            }
        }
    }
}