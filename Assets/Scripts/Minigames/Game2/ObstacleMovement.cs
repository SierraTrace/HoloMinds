using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    
    public float speed = 5f;
    public float leftLimit = -15f;

        
    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x < leftLimit)
        {
            Destroy(gameObject);
        }
    }
}
