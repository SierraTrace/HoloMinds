using UnityEngine;

public class TransitionAudioManager : MonoBehaviour
{
    public static TransitionAudioManager Instance;


    [Header("Fuentes de Audio")]
    public AudioSource AudioSource;


    [Header("Clips de Audio")]
    public AudioClip swooshSound;
    public AudioClip tickSound;
    public AudioClip goSound;


    private void Awake()
    {
        Instance = this;
    }

    public void PlaySwoosh()
    {
        if (swooshSound != null) AudioSource.PlayOneShot(swooshSound);
    }

    public void PlayTick()
    {
        if (tickSound != null) AudioSource.PlayOneShot(tickSound);
    }

    public void PlayGo()
    {
        if (goSound != null) AudioSource.PlayOneShot(goSound);
    }

}
