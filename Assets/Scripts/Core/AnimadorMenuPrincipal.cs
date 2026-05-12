using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimadorMenuPrincipal : MonoBehaviour
{
    [Header("Objetos de referencia")]
    [SerializeField] private TextMeshProUGUI tituloTexto;
    [SerializeField] private SpriteRenderer imagenChica;
    [SerializeField] private GameObject botonJugar;
    [SerializeField] private GameObject botonSalir;

    [Header("Configuración de Animación")]
    [SerializeField] private float duracionFade = 1.5f;
    [SerializeField] private float duracionMovimiento = 1.0f;
    [SerializeField] private float pausaEntrePasos = 0.4f;


    private Vector3 positionFinalImage;
    private float offsetFueraDePantalla = 1000f;


    private void Start()
    {
        positionFinalImage = imagenChica.transform.localPosition;

        // Ocultar elementos al inicio
        PrepararEscena();

        StartCoroutine(SecuenciaAparicion());
    }


    private void PrepararEscena()
    {
        // Titulo transparente
        Color c = tituloTexto.color;
        c.a = 0;
        tituloTexto.color = c;

        // Imagen fuera de pantalla
        imagenChica.transform.localPosition = new Vector3(
            positionFinalImage.x + offsetFueraDePantalla,
            positionFinalImage.y,
            positionFinalImage.z
        );

        // Botones ocultos
        botonJugar.SetActive(false);
        botonSalir.SetActive(false);
    }


    private IEnumerator SecuenciaAparicion()
    {
        // Titulo
        float tiempo = 0;
        while (tiempo < duracionFade)
        {
            tiempo += Time.deltaTime;
            Color c = tituloTexto.color;
            c.a = Mathf.Lerp(0, 1, tiempo / duracionFade);
            tituloTexto.color = c;
            yield return null;
        }

        yield return new WaitForSeconds(pausaEntrePasos);


        // Imagen
        tiempo = 0;
        Vector3 posicionInicial = imagenChica.transform.localPosition;

        while (tiempo < duracionMovimiento)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionMovimiento;
            t = t * t * (3f - 2f * t);

            imagenChica.transform.localPosition = Vector3.Lerp(posicionInicial, positionFinalImage, t);
            yield return null;
        }

        yield return new WaitForSeconds(pausaEntrePasos);

        // Botones
        botonJugar.SetActive(true);
        botonSalir.SetActive(true);
    }

}
