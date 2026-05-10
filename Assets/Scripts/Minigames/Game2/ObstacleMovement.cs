using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    public float leftLimit = -15f;

    public float synchronizationFactor = 20f;            // Factor para ajustar la sincronización con el parallax  


    void Update()
    {
        float global = ParallaxController.currentWorldSpeed;
        float road = ParallaxController.roadSpeedReference;

        float velocity = global * road * synchronizationFactor;

        transform.position += Vector3.left * velocity * Time.deltaTime;

        if (transform.position.x < leftLimit)
        {
            Destroy(gameObject);
        }

    }
}
