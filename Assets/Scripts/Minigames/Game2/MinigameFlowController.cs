using UnityEngine;
using System.Collections;

public enum  EndType { Death, Timeout}

public class MinigameFlowController : MonoBehaviour
{
    public static MinigameFlowController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void EndGame(int finalScore, Type)
    {
        StartCoroutine(EndSequence(finalScore, type));
    }

    private IEnumerator EndSequence(int finalScore, EndType type)
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.StopScore();
        }

        ParallaxController parallax = FindObjectOfType<ParallaxController>();
        if (parallax != null)
        {
            parallax.globalSpeed = 0;
        }

        ObstacleSpawner spawner = FindObjectOfType<ObstacleSpawner>();
        if (spawner != null)
        {
            spawner.StopAllCoroutines();
        }

        foreach (var obstacle in FindObjectsOfType<ObstacleMovement>())
        {
            obstacle.enabled = false;
        }

        PlayerJump player = FindObjectOfType<PlayerJump>();
        if (player != null)
        {
            player.enabled = false;
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            if (type == EndType.Death)
            {
                player.GetComponent<Animator>().SetTrigger("Die");
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
    }
}
