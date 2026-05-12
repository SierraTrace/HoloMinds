using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.CompilerServices;

public class MainMenuAnimator : MonoBehaviour
{
    [Header("Objetos de referencia")]
    [SerializeField] private TextMeshProUGUI tituloTexto;
    [SerializeField] private SpriteRenderer imagenChica;
    [SerializeField] private CanvasGroup grupoBotones;

    [Header("Configuración de Animación")]
    [SerializeField] private float duracionFade = 1.5f;
    [SerializeField] private float duracionMovimiento = 1.0f;
    [SerializeField] private float pausaEntrePasos = 0.4f;


    private Vector3 positionFinalImage;
    private float offsetFueraDePantalla = 1500f;


    private void Start()
    {
        
        if (tituloTexto == null || imagenChica == null || grupoBotones == null)
        {
            Debug.LogError("Faltan referencias en el MainMenuAnimator. Asegúrate de asignar todos los objetos en el inspector.");
            return;
        }

        positionFinalImage = imagenChica.transform.localPosition;

        // Ocultar elementos al inicio
        PrepararEscena();

        StartCoroutine(SecuenciaAparicion());

        Invoke("IniciarMusica", 0.1f);
    }

    private void IniciarMusica()
    {
        if (MenuAudioManager.Instance != null)
        {
            MenuAudioManager.Instance.FadeInMusica(duracionFade, 0.5f);
        }
    }


    private void PrepararEscena()
    {
        // Titulo transparente
        tituloTexto.alpha = 0;

        // Imagen fuera de pantalla
        imagenChica.transform.localPosition = positionFinalImage + new Vector3(offsetFueraDePantalla, 0, 0);

        // Botones ocultos
        grupoBotones.alpha = 0;
        grupoBotones.interactable = false;
        grupoBotones.blocksRaycasts = false;
    }


    private IEnumerator SecuenciaAparicion()
    {
        // Titulo
        yield return StartCoroutine(FadeTexto(0, 1, duracionFade));
        yield return new WaitForSeconds(pausaEntrePasos);


        // Imagen
        float tiempo = 0;
        Vector3 posicionInicial = imagenChica.transform.localPosition;

        while (tiempo < 1)
        {
            tiempo += Time.deltaTime / duracionMovimiento;
            float tSuave = 1 - Mathf.Pow(1 - tiempo, 3);
            imagenChica.transform.localPosition = Vector3.Lerp(posicionInicial, positionFinalImage, tSuave);
            yield return null;
        }

        yield return new WaitForSeconds(pausaEntrePasos);


        // Botones
        float tBotones = 0;
        while (tBotones < 1)
        {
            tBotones += Time.deltaTime / duracionFade;
            grupoBotones.alpha = tBotones;
            yield return null;
        }

        grupoBotones.interactable = true;
        grupoBotones.blocksRaycasts = true;
    }



    private IEnumerator FadeTexto(float alphaInicial, float alphaFinal, float duracion)
    {
        float tiempo = 0;
        while (tiempo < 1)
        {
            tiempo += Time.deltaTime / duracion;
            tituloTexto.alpha = Mathf.Lerp(alphaInicial, alphaFinal, tiempo);
            yield return null;
        }
    }


    public void DesactivarBotones()
    {
        grupoBotones.interactable = false;
        grupoBotones.blocksRaycasts = false;
       
        StartCoroutine(FadeBotonesSalida());
    }

    private IEnumerator FadeBotonesSalida()
    {
        while (grupoBotones.alpha > 0)
        {
            grupoBotones.alpha -= Time.deltaTime * 2f;
            yield return null;
        }
    }
}
