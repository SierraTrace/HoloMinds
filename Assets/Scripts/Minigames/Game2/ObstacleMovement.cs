using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    public float leftLimit = -15f;

        
    void Update()
    {
        float currentSpeed = ScoreManager.Instance.GetWorldSpeed();
        
        transform.position += Vector3.left * currentSpeed * Time.deltaTime;

        if (transform.position.x < leftLimit)
        {
            Destroy(gameObject);
        }
    }
}
