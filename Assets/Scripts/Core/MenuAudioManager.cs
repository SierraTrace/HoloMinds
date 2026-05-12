using UnityEngine;
using System.Collections;

public class MenuAudioManager : MonoBehaviour
{
    public static MenuAudioManager Instance;

    [Header("Audio")]
    [SerializeField] private AudioSource musicaSource;
    [SerializeField] private AudioClip musicClip;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (musicaSource != null && musicClip != null)
        {
            musicaSource.clip = musicClip;
            musicaSource.loop = true;
            musicaSource.Play();
            FadeInMusica(1.5f, 1f);
        }
    }


    public void FadeInMusica(float duracion, float volumenObjetivo)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(DoFade(musicaSource.volume, volumenObjetivo, duracion));
    }

    public void FadeOutMusica(float duracion)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(DoFade(musicaSource.volume, 0, duracion));
    }

    private IEnumerator DoFade(float inicio, float fin, float duracion)
    {
        if (musicaSource == null) yield break;
        if (!musicaSource.isPlaying && fin > 0) musicaSource.Play();

        float tiempo = 0;
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            musicaSource.volume = Mathf.Lerp(inicio, fin, tiempo / duracion);
            yield return null;
        }

        musicaSource.volume = fin;
        if (fin == 0) musicaSource.Stop();
    }
}