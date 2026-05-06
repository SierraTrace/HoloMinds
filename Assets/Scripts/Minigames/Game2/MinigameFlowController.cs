using UnityEngine;
using System.Collections;

public enum  EndType
{
    Death,
    Timeout
}

public class MinigameFlowController : MonoBehaviour
{
    public static MinigameFlowController Instance;

    private void Awake()
    {
        Instance = this;
    }

    private bool gameEnded = false;

    public void EndGame(int finalScore, EndType type)
    {
        if (gameEnded) return;
        
        gameEnded = true;
        StartCoroutine(EndSequence(finalScore, type));
        
    }

    private IEnumerator EndSequence(int finalScore, EndType type)
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.StopScore();
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBackgroundMusic();
        }

        ParallaxController parallax = FindObjectOfType<ParallaxController>();
        if (parallax != null)
        {
            parallax.globalSpeed = 0;
        }

        foreach (var obstacle in FindObjectsOfType<ObstacleMovement>())
        {
            obstacle.enabled = false;
        }

        ObstacleSpawner spawner = FindObjectOfType<ObstacleSpawner>();
        if (spawner != null)
        {
            spawner.StopAllCoroutines();
        }

        PlayerJump player = FindObjectOfType<PlayerJump>();
        if (player != null)
        {
            player.enabled = false;
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.simulated = false;
            }

            if (type == EndType.Death)
            {
                player.GetComponent<Animator>().SetTrigger("die");

                CameraShake shaker = Camera.main.GetComponent<CameraShake>();
                if (shaker != null)
                {
                    shaker.Shake(0.4f, 0.3f);
                }
                else
                {
                    Debug.LogWarning("No CameraShaker found on main camera.");
                }
            }
        }

        if (type == EndType.Death)
        {
            yield return new WaitForSeconds(1.5f);
        }

        MinigameEnd end = FindObjectOfType<MinigameEnd>();
        if (end != null)
        {
            end.FinishMinigameWithScore(finalScore);
        }
        else
        {
            Debug.LogWarning("No MinigameEnd found in scene.");
        }
    }
}
