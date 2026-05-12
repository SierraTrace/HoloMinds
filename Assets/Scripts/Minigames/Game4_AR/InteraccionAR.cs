using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class InteraccionAR : MonoBehaviour
{
    [HideInInspector] public int puntosLocales = 0;

    [Header("Interfaz")]
    public Slider barraPuntos;
    public TextMeshProUGUI textoScore;
    public int puntosMaximos = 100;
    public int puntosPorObjeto = 10;

    [Header("Efectos Visuales")]
    public GameObject prefabParticulasBuenas;
    public GameObject prefabParticulasMalas;

    void Start()
    {
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
                    if (objetoTocado.debeElimibarse)
                    {
                        puntosLocales += puntosPorObjeto;

                        if (puntosLocales >= puntosMaximos)
                        {
                            ControladorTiempoAR timer = FindFirstObjectByType<ControladorTiempoAR>();
                            if (timer != null) timer.TerminarMicrojuego();
                        }

                        if (prefabParticulasBuenas != null)
                            Instantiate(prefabParticulasBuenas, hit.transform.position, Quaternion.identity);
                    }
                    else
                    {
                        puntosLocales -= puntosPorObjeto;
                        if (puntosLocales < 0) puntosLocales = 0;

                        if (prefabParticulasMalas != null)
                            Instantiate(prefabParticulasMalas, hit.transform.position, Quaternion.identity);
                    }

                    if (barraPuntos != null) barraPuntos.value = puntosLocales;
                    if (textoScore != null) textoScore.text = puntosLocales.ToString();

                    Destroy(hit.transform.gameObject);
                    break;
                }
            }
        }
    }
}
