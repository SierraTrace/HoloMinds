using UnityEngine;
using TMPro;
using System.Collections;


public class TransitionManager : MonoBehaviour
{
    [Header("Referencias de UI")]
    public RectTransform titleText;
    public RectTransform descriptionText;
    public RectTransform countdownText;

    [Header("Configuración de animación")]
    public float animationDuration = 1f;
    public Vector2 titleTargetPos = new Vector2(0, 100);
    public Vector2 descTargetPos = new Vector2(0, 0);

    [Header("Textos del Nivel")]
    public string levelTitle = "Minijuego 1";
    public string levelDescription = "Atrapa todos los corazones rotos";
    

    void Start()
    {
        
        titleText.GetComponent<TextMeshProUGUI>().text = levelTitle;
        descriptionText.GetComponent<TextMeshProUGUI>().text = levelDescription;
        countdownText.GetComponent<TextMeshProUGUI>().text = "";

        titleText.anchoredPosition = new Vector2(-1500, titleTargetPos.y);
        descriptionText.anchoredPosition = new Vector2(1500, descTargetPos.y);

        StartCoroutine(TransitionSequence());

    }



    IEnumerator TransitionSequence()
    {

        float elapsed = 0f;

        // Animar el título y la descripción hacia sus posiciones objetivo
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            float curve = t * t * (3f - 2f * t);

            titleText.anchoredPosition = Vector2.Lerp(new Vector2(-1500, titleTargetPos.y), titleTargetPos, curve);
            descriptionText.anchoredPosition = Vector2.Lerp(new Vector2(1500, descTargetPos.y), descTargetPos, curve);

            yield return null;
        }

        // Cuenta atrás
        int remainingTime = 5;
        while (remainingTime > 0)
        {
            countdownText.GetComponent<TextMeshProUGUI>().text = remainingTime.ToString();

            countdownText.localScale = Vector3.one * 1.5f;

            yield return new WaitForSeconds(1f);
            remainingTime--;
        }

        countdownText.GetComponent<TextMeshProUGUI>().text = "¡GO!";
        countdownText.localScale = Vector3.one * 2f;
    
        yield return new WaitForSeconds(1f);

        // TODO: Cargar siguiente mnijuego
        // SceneManager.LoadScene("NombreDeTuMinijuego");

    }

}
