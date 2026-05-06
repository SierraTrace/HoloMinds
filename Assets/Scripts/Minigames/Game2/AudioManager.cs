using UnityEngine;
using System.Collections;
using UnityEditor.Experimental.GraphView;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Configuración de Música")]
    public AudioSource musicSource;
    public AudioClip backgroundMusic;
    public float maxMusicVolume = 0.5f;
    public float fadeInDuration = 2.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
        }

        if (musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.volume = 0f;            // Ajustado de inicio a 0 para el fade-in  
        }
    }

    private void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.Play();
            StartCoroutine(FadeInMusicRoutine());
        }
        else
        {
            Debug.LogWarning("AudioManager: No se puede reproducir la música. Falta AudioSource o AudioClip.");
        }
    }

    public void StopBackgroundMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }


    private IEnumerator FadeInMusicRoutine()
    {
        float timer = 0f;
        musicSource.volume = 0f;

        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, maxMusicVolume, timer / fadeInDuration);
            yield return null; // Esperamos el siguiente frame
        }

        musicSource.volume = maxMusicVolume;
    }











}
