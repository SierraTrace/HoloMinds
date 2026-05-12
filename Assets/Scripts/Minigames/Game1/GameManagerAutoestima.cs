using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManagerAutoestima : MonoBehaviour // Nombre ÚNICO para tu juego
{
    public static GameManagerAutoestima instance; // Instancia ÚNICA

    [Header("Ajustes del GDD")]
    public float autoestima = 5f;
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
    [SerializeField] private AudioSource fuenteAudio;
    public static ScoreManager Instance;    // Singleton instance

    [Header("Configuration")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public float gameSpeed = 5f;
    public float gameDuration = 30f;        // Duraci�n total del minijuego
    public BonusAnimator bonusAnimator;
    public Transform scoreTargetPosition;
    [Tooltip("Wire in editor if possible.")]
    public MinigameEnd end = null;

    [Header("Feedback Visual")]
    public Color warningcolor = Color.red;
    public float blinkInterval = 0.5f;

    private float _timer;
    private bool _isPaused = false;
    private bool _isWarningActive = false;
    private Color _originalColor;
    void Awake()
    {
        // Configuramos tu propia instancia
        instance = this;

        // Buscamos el AudioSource en el objeto donde esté este script
        //fuenteAudio = GetComponent<AudioSource>();

        if (fuenteAudio != null)
        {
            fuenteAudio.spatialBlend = 0;
            //fuenteAudio.volume = 1.0f;
            fuenteAudio.playOnAwake = false;
        }
    }
    private void Start()
    {
        _timer = gameDuration;

        if (timerText != null)
        {
            _originalColor = timerText.color;
        }

        //float maxScore = gameSpeed * gameDuration;

        // Establecer el valor m�ximo en la barra de autoestima
        //if (SelfEsteemBar.Instance != null)
        //{
        //    SelfEsteemBar.Instance.SetMaxValue(maxScore);
        //}
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

        if (tiempoRestante <= 0) FinDelJuego(autoestima >= 5);

        if (_isPaused) return;

        _timer -= Time.deltaTime;

        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(_timer);
            timerText.text = seconds.ToString() + 's';
        }

        if (_timer <= 5f && !_isWarningActive)
        {
            _isWarningActive = true;
            StartCoroutine(BlinkTimerRoutine());
        }


        UpdateScoreUI();

    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = (autoestima * 10).ToString() + " pts";
        }
        barraAutoestima.value = autoestima;
        barraAutoestima.Rebuild(CanvasUpdate.PreRender);
    }


    public void ModificarAutoestima(int cantidad)
    {
        if (!juegoActivo) return;

        autoestima += cantidad;
        autoestima = Mathf.Clamp(autoestima, 0, 10);

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


    private IEnumerator BlinkTimerRoutine()
    {
        while (_timer > 0 && !_isPaused)
        {
            timerText.color = warningcolor;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayTimerWarning();
            }

            yield return new WaitForSeconds(blinkInterval);

            timerText.color = _originalColor;
            yield return new WaitForSeconds(blinkInterval);
        }

        timerText.color = _originalColor;
    }

    // Metodo de cambio de escena con botón, para pruebas
    /*
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
    }*/

    void FinDelJuego(bool ganado)
    {
        if (!juegoActivo) return;
        juegoActivo = false;

        panelGameOver.SetActive(true);

        if (barraAutoestima != null) barraAutoestima.value = autoestima;

        if (textoResultado != null)
            textoResultado.text = ganado ? mensajeVictoria : mensajeDerrota;

        StartCoroutine(EsperarYPasarDeNivel());
    }

    System.Collections.IEnumerator EsperarYPasarDeNivel()
    {
        yield return new WaitForSeconds(1f);    // Espera para que el jugador vea el resultado

        // MinigameEnd endScript = Object.FindFirstObjectByType<MinigameEnd>();

        //if (end != null)
        ////    int puntuacionFinal = Mathf.RoundToInt(autoestima);
        //   end.FinishMinigameWithScore(puntuacionFinal);
        //}
        //else
        //{
        Debug.LogError("No se encontró el script MinigameEnd en la escena.");
        //  }

        SendFinalScore();
    }

    private void SendFinalScore()
    {
        int finalScore = (int)(autoestima * 10); // Convertimos la autoestima a una puntuación (ejemplo: 0-10 -> 0-100)  
        Debug.Log($"Puntuación final de {finalScore} enviada al MinigameEnd.");
        if (end == null)
        {
            end = FindObjectOfType<MinigameEnd>();
            Debug.LogWarning("MinigameEnd no asignado en el inspector, buscando en la escena...");
        }

        if (end != null)
        {
            end.FinishMinigameWithScore(finalScore);
            Debug.Log("Puntuación enviada al MinigameEnd.");
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