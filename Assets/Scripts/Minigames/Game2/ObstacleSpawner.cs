using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Pool de Obstáculos")]
    public GameObject[] obstaclePrefabs;  // Array de prefabs para diferentes tipos de obstáculos

    [Header("Configuración de Spawn")]
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 3f;


    void Start()
    {
        if (obstaclePrefabs.Length > 0)
        {
            StartCoroutine(SpawnRoutine());
        }
        else
        {
            Debug.LogError("ObstacleSpawner: No hay prefabs asignados en el array");
        }
    }


    private System.Collections.IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            if (ParallaxController.currentWorldSpeed > 0)
            {
                SpawnRandomObstacle();
            }
        }
    }

    private void SpawnRandomObstacle()
    {
        int randomIndex = Random.Range(0, obstaclePrefabs.Length);
        Instantiate(obstaclePrefabs[randomIndex], transform.position, Quaternion.identity);
    }

}
