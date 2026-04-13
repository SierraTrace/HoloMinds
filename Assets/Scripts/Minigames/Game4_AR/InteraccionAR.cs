using UnityEngine;
using UnityEngine.InputSystem; 

public class InteraccionAR : MonoBehaviour
{
    [HideInInspector] public int puntosLocales = 0; 

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
                    if (objetoTocado.debeElimibarse)
                        puntosLocales += objetoTocado.valorPuntos;
                    else
                        puntosLocales -= objetoTocado.valorPuntos;

                    Destroy(hit.transform.gameObject);
                    break; 
                }
            }
        }
    }
}