using UnityEngine;
using TMPro;
using System.Collections;


public class TransitionManager : MonoBehaviour
{
    [Header("Referencias de UI")]
    public RectTransform titleText;
    public RectTransform descriptionText;
    public RectTransform countdownText;

    [Header("Configuraci�n de animaci�n")]
    public float animationDuration = 1f;
    public Vector2 titleTargetPos = new Vector2(0, 100);
    public Vector2 descTargetPos = new Vector2(0, 0);

    [Header("Textos del Nivel")]
    public string levelTitle = "MINIJUEGO X";
    public string levelDescription = "ATRAPA TODOS LOS CORAZONES ROTOS";
    

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
        

        // Animar el t�tulo y la descripci�n hacia sus posiciones objetivo
        while (elapsed < animationDuration)
        {

            if (elapsed == 0 && TransitionAudioManager.Instance != null)
            {
                TransitionAudioManager.Instance.PlaySwoosh();
            }


            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            float curve = t * t * (3f - 2f * t);

            titleText.anchoredPosition = Vector2.Lerp(new Vector2(-1500, titleTargetPos.y), titleTargetPos, curve);
            descriptionText.anchoredPosition = Vector2.Lerp(new Vector2(1500, descTargetPos.y), descTargetPos, curve);

            yield return null;
        }

        yield return new WaitForSeconds(1f);

        // Cuenta atr�s
        int remainingTime = 5;
        while (remainingTime > 0)
        {
            countdownText.GetComponent<TextMeshProUGUI>().text = remainingTime.ToString();
            countdownText.localScale = Vector3.one * 1.5f;

            if (TransitionAudioManager.Instance != null)
            {
                TransitionAudioManager.Instance.PlayTick();
            }

            yield return new WaitForSeconds(1f);
            remainingTime--;
        }

        countdownText.GetComponent<TextMeshProUGUI>().text = "GO!";
        countdownText.localScale = Vector3.one * 2f;

        if (TransitionAudioManager.Instance != null)
        {
            TransitionAudioManager.Instance.PlayGo();
        }
    
        yield return new WaitForSeconds(0.6f);

        SceneLoader.LoadNextScene();

    }

}
