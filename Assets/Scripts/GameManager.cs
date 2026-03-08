using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // VITAL para poder reiniciar la escena
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Ajustes del GDD")]
    public float autoestima = 20f; 
    public float tiempoRestante = 10f;
    public bool juegoActivo = true;
    public string mensajeVictoria = "¡Felicidades, has ganado autoestima!";
    public string mensajeDerrota = "¡Deja ya de pensar en tu ex!";



    [Header("Referencias UI")]
    public Slider barraAutoestima;
    public TextMeshProUGUI textoTimer;
    public GameObject panelGameOver;
    public TextMeshProUGUI textoResultado; // Para poner "¡Ganaste!" o "¡Perdiste!"
    
    [Header("Referencias Visuales")]
    public SpriteRenderer cerebroRenderer; 

    void Awake() 
    { 
        instance = this; 
    }

    void Update()
{
    if (!juegoActivo) return;

    tiempoRestante -= Time.deltaTime;
    if (textoTimer != null)
        textoTimer.text = Mathf.Ceil(tiempoRestante).ToString();

    if (tiempoRestante <= 0) FinDelJuego(autoestima > 10);

    // FEEDBACK SENIOR: para que la barra suba suavemente
    if (barraAutoestima != null)
    {
        // Esto hace que la barra "persiga" al valor de autoestima suavemente
        barraAutoestima.value = Mathf.Lerp(barraAutoestima.value, autoestima, Time.deltaTime * 5f);
    }
    // hace que la barra de autoestima suba suavemente
    if (barraAutoestima != null)
    {
        barraAutoestima.value = Mathf.Lerp(barraAutoestima.value, autoestima, Time.deltaTime * 5f);
    }
}
    public void ModificarAutoestima(int cantidad)
    {
        if (!juegoActivo) return;
        
        autoestima += cantidad;
        autoestima = Mathf.Clamp(autoestima, 0, 100);

        // Feedback visual del cerebro (Punto 4.2 GDD)
        StopAllCoroutines();
        if (cantidad > 0) StartCoroutine(FeedbackCerebro(Color.white)); 
        else StartCoroutine(FeedbackCerebro(Color.gray)); 
        
        // Si llegas a 0 de autoestima, pierdes inmediatamente
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

        if (textoResultado != null)
        {
            textoResultado.text = ganado ? mensajeVictoria : mensajeDerrota;
        }
    }

   
    public void Reiniciar()
    {
        // Carga de nuevo la escena en la que estamos
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}