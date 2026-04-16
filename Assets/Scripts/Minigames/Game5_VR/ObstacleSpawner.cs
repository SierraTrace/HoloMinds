using UnityEngine;
using System.Collections.Generic;

// Gestiona el spawn de obstáculos (Ex) frente al jugador
// Configurable para ajustar dificultad, frecuencia y patrones
public class ObstacleSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("Prefab del obstáculo a spawnear")]
    public GameObject obstaclePrefab;

    [Header("Spawn Points")]
    [Tooltip("Puntos desde donde aparecen los obstáculos")]
    public Transform[] spawnPoints;
    
    [Tooltip("Si no hay spawn points, usa esta distancia")]
    public float spawnRadius = 20f;
    
    [Tooltip("Altura de spawn si se genera automáticamente")]
    public float spawnHeight = 1.5f;

    [Header("Timing")]
    [Tooltip("Tiempo entre spawns (segundos)")]
    public float spawnInterval = 2f;
    
    [Tooltip("Variación aleatoria del intervalo")]
    public float intervalVariance = 0.5f;
    
    [Tooltip("Reduce el intervalo con el tiempo (más difícil)")]
    public float difficultyRamp = 0.02f;

    [Header("Dificultad")]
    [Tooltip("Velocidad inicial de los obstáculos")]
    public float baseSpeed = 3f;
    
    [Tooltip("Velocidad máxima de los obstáculos")]
    public float maxSpeed = 10f;
    
    [Tooltip("Incremento de velocidad por obstáculo spawneado")]
    public float speedIncrement = 0.1f;

    [Header("Object Pooling")]
    [Tooltip("Cantidad inicial de obstáculos en el pool")]
    public int poolSize = 20;

    [Header("Estado")]
    [SerializeField] private bool isSpawning = false;
    [SerializeField] private int obstaclesSpawned = 0;
    [SerializeField] private float currentSpeed;

    private List<Obstacle> obstaclePool = new List<Obstacle>();
    private float nextSpawnTime;
    private Transform playerTransform;

    // Evento para notificar cuando un obstáculo da resultado
    public System.Action<bool> OnObstacleResult;

    void Start()
    {
        playerTransform = Camera.main?.transform;
        currentSpeed = baseSpeed;
        InitializePool();
    }

    void Update()
    {
        if (isSpawning && Time.time >= nextSpawnTime)
        {
            SpawnObstacle();
            ScheduleNextSpawn();
        }
    }

    // Inicializa el pool de obstáculos
    void InitializePool()
    {
        if (obstaclePrefab == null)
        {
            Debug.LogError("ObstacleSpawner: No hay prefab de obstáculo asignado!");
            return;
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(obstaclePrefab, Vector3.zero, Quaternion.identity, transform);
            obj.SetActive(false);
            
            Obstacle obstacle = obj.GetComponent<Obstacle>();
            if (obstacle != null)
            {
                obstacle.target = playerTransform;
                obstacle.OnObstacleResult += HandleObstacleResult;
                obstaclePool.Add(obstacle);
            }
        }

        Debug.Log($"ObstacleSpawner: Pool inicializado con {poolSize} obstáculos");
    }

    // Obtiene un obstáculo del pool o crea uno nuevo si es necesario
    Obstacle GetFromPool()
    {
        foreach (Obstacle obs in obstaclePool)
        {
            if (!obs.gameObject.activeInHierarchy)
            {
                return obs;
            }
        }

        if (obstaclePrefab != null)
        {
            GameObject obj = Instantiate(obstaclePrefab, Vector3.zero, Quaternion.identity, transform);
            Obstacle obstacle = obj.GetComponent<Obstacle>();
            if (obstacle != null)
            {
                obstacle.target = playerTransform;
                obstacle.OnObstacleResult += HandleObstacleResult;
                obstaclePool.Add(obstacle);
                return obstacle;
            }
        }

        return null;
    }

    // Spawnea un obstáculo en una posición
    void SpawnObstacle()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Obstacle obstacle = GetFromPool();

        if (obstacle != null)
        {
            obstacle.ResetObstacle(spawnPosition, currentSpeed);
            obstaclesSpawned++;
            currentSpeed = Mathf.Min(currentSpeed + speedIncrement, maxSpeed);
        }
    }

    // Obtiene una posición para spawnear FRENTE al jugador
    // El Ex siempre aparece de frente y avanza hacia el jugador
    Vector3 GetRandomSpawnPosition()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            return spawnPoints[randomIndex].position;
        }

        if (playerTransform != null)
        {
            // Genera posición frente al jugador con variación lateral pequeña
            float angleVariation = Random.Range(-30f, 30f) * Mathf.Deg2Rad;
            
            float x = Mathf.Sin(angleVariation) * spawnRadius;
            float z = Mathf.Cos(angleVariation) * spawnRadius;
            
            return new Vector3(
                playerTransform.position.x + x,
                spawnHeight,
                playerTransform.position.z + z
            );
        }

        return new Vector3(0, spawnHeight, spawnRadius);
    }

    // Programa el siguiente spawn
    void ScheduleNextSpawn()
    {
        float variance = Random.Range(-intervalVariance, intervalVariance);
        float currentInterval = Mathf.Max(0.5f, spawnInterval - (obstaclesSpawned * difficultyRamp));
        nextSpawnTime = Time.time + currentInterval + variance;
    }

    // Maneja el resultado de un obstáculo
    void HandleObstacleResult(bool wasDodged)
    {
        OnObstacleResult?.Invoke(wasDodged);
    }

    // Inicia el spawning de obstáculos
    public void StartSpawning()
    {
        isSpawning = true;
        nextSpawnTime = Time.time + 1f;
        Debug.Log("ObstacleSpawner: Inicia el spawning");
    }

    // Detiene el spawning
    public void StopSpawning()
    {
        isSpawning = false;
        Debug.Log("ObstacleSpawner: Detiene el spawning");
    }

    // Resetea el spawner para una nueva partida
    public void ResetSpawner()
    {
        StopSpawning();
        obstaclesSpawned = 0;
        currentSpeed = baseSpeed;

        foreach (Obstacle obs in obstaclePool)
        {
            obs.Deactivate();
        }
    }

    // Desactiva todos los obstáculos activos
    public void ClearAllObstacles()
    {
        foreach (Obstacle obs in obstaclePool)
        {
            if (obs.gameObject.activeInHierarchy)
            {
                obs.Deactivate();
            }
        }
    }
}
