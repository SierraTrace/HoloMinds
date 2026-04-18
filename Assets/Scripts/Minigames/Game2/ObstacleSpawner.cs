using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    
    public GameObject obstaclePrefab;
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 3f;


    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private System.Collections.IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);
            Instantiate(obstaclePrefab, transform.position, Quaternion.identity);
        }
    }

}
