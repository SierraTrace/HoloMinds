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

    [Header("Configuración de SFX")]
    public AudioSource sfxSource;

    [Header("Clips de audio SFX")]
    public AudioClip jumpSound;


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

        if (sfxSource == null)
        {
            AudioSource[] sources = GetComponents<AudioSource>();
            if (sources.Length > 1)
            {
                sfxSource = sources[1];
            }
            else
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
            }
        }

        if (sfxSource != null)
        {
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
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

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("AudioManager: No se puede reproducir el SFX. Falta AudioSource o AudioClip.");
        }
    }

    public void PlayJumpSound()
    {
        if (jumpSound != null)
        {
            PlaySFX(jumpSound);
        }
        else
        {
            Debug.LogWarning("AudioManager: No se puede reproducir el sonido de salto. Falta AudioClip.");
        }
    }



}
