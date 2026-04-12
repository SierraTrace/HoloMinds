using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManagerAutoestima : MonoBehaviour // Nombre ÚNICO para tu juego
{
    public static GameManagerAutoestima instance; // Instancia ÚNICA

    [Header("Ajustes del GDD")]
    public float autoestima = 10f;
    public float tiempoRestante = 10f;
    public bool juegoActivo = true;
    public string mensajeVictoria = "¡Felicidades, has ganado autoestima!";
    public string mensajeDerrota = "¡Deja ya de pensar en tu ex!";

    [Header("Referencias UI")]
    public Slider barraAutoestima;
    public TextMeshProUGUI textoTimer;
    public GameObject panelGameOver;
    public TextMeshProUGUI textoResultado;

    [Header("Referencias Visuales")]
    public SpriteRenderer cerebroRenderer;

    [Header("Sonidos")]
    public AudioClip sonidoAcierto;
    public AudioClip sonidoFallo;
    private AudioSource fuenteAudio;

    void Awake()
    {
        // Configuramos tu propia instancia
        instance = this;

        // Buscamos el AudioSource en el objeto donde esté este script
        fuenteAudio = GetComponent<AudioSource>();

        if (fuenteAudio != null)
        {
            fuenteAudio.spatialBlend = 0;
            fuenteAudio.volume = 1.0f;
            fuenteAudio.playOnAwake = false;
        }
    }

    void Update()
    {
        if (barraAutoestima != null)
        {
            barraAutoestima.value = Mathf.Lerp(barraAutoestima.value, autoestima, Time.deltaTime * 5f);
        }

        if (!juegoActivo) return;

        tiempoRestante -= Time.deltaTime;
        if (textoTimer != null)
            textoTimer.text = Mathf.Ceil(tiempoRestante).ToString();

        if (tiempoRestante <= 0) FinDelJuego(autoestima > 10);
    }

    public void ModificarAutoestima(int cantidad)
    {
        if (!juegoActivo) return;

        autoestima += cantidad;
        autoestima = Mathf.Clamp(autoestima, 0, 100);

        StopAllCoroutines();
        if (cantidad > 0) StartCoroutine(FeedbackCerebro(Color.white));
        else StartCoroutine(FeedbackCerebro(Color.gray));

        if (autoestima <= 0 && cantidad < 0) FinDelJuego(false);
    }

    System.Collections.IEnumerator FeedbackCerebro(Color colorEfecto)
    {
        if (cerebroRenderer == null) yield break;
        cerebroRenderer.color = colorEfecto;
        yield return new WaitForSeconds(0.5f);
        cerebroRenderer.color = Color.white;
    }

    void FinDelJuego(bool ganado)
    {
        juegoActivo = false;
        panelGameOver.SetActive(true);

        if (barraAutoestima != null) barraAutoestima.value = autoestima;

        if (textoResultado != null)
            textoResultado.text = ganado ? mensajeVictoria : mensajeDerrota;

        // --- CONEXIÓN CON EL SISTEMA BASE ---

        // 1. Buscamos el script MinigameEnd que está en el objeto de la escena
        MinigameEnd endScript = Object.FindFirstObjectByType<MinigameEnd>();

        if (endScript != null)
        {
            // 2. Le pasamos tu puntuación real (convertida a entero)
            // Sustituimos el "testScore" por mi "autoestima"
            endScript.testScore = Mathf.RoundToInt(autoestima);

            Debug.Log($"Puntuación final de {autoestima} enviada al MinigameEnd.");
        }
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PlaySonido(bool esBueno)
    {
        AudioClip clip = esBueno ? sonidoAcierto : sonidoFallo;
        if (clip != null && fuenteAudio != null)
        {
            fuenteAudio.PlayOneShot(clip);
        }
    }
}