using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gestiona el spawn de obstáculos desde diferentes posiciones.
/// Configurable para ajustar dificultad, frecuencia y patrones.
/// </summary>
public class ObstacleSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("Prefab del obstáculo a spawnear")]
    public GameObject obstaclePrefab;

    [Header("Spawn Points")]
    [Tooltip("Puntos desde donde pueden aparecer los obstáculos")]
    public Transform[] spawnPoints;
    
    [Tooltip("Si no hay spawn points, usar esta distancia alrededor del jugador")]
    public float spawnRadius = 20f;
    
    [Tooltip("Altura de spawn si se genera automáticamente")]
    public float spawnHeight = 1.5f;

    [Header("Timing")]
    [Tooltip("Tiempo entre spawns (segundos)")]
    public float spawnInterval = 2f;
    
    [Tooltip("Variación aleatoria del intervalo")]
    public float intervalVariance = 0.5f;
    
    [Tooltip("Reducir intervalo con el tiempo (más difícil)")]
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
    public System.Action<bool> OnObstacleResult; // true = esquivado, false = golpeado

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

    /// <summary>
    /// Inicializa el pool de obstáculos
    /// </summary>
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

    /// <summary>
    /// Obtiene un obstáculo del pool o crea uno nuevo si es necesario
    /// </summary>
    Obstacle GetFromPool()
    {
        foreach (Obstacle obs in obstaclePool)
        {
            if (!obs.gameObject.activeInHierarchy)
            {
                return obs;
            }
        }

        // Si no hay disponibles, crear uno nuevo
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

    /// <summary>
    /// Spawnea un obstáculo en una posición aleatoria
    /// </summary>
    void SpawnObstacle()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Obstacle obstacle = GetFromPool();

        if (obstacle != null)
        {
            obstacle.ResetObstacle(spawnPosition, currentSpeed);
            obstaclesSpawned++;

            // Incrementar dificultad
            currentSpeed = Mathf.Min(currentSpeed + speedIncrement, maxSpeed);
        }
    }

    /// <summary>
    /// Obtiene una posición para spawnear FRENTE al jugador (según GDD)
    /// El Ex siempre aparece de frente y avanza hacia el jugador
    /// </summary>
    Vector3 GetRandomSpawnPosition()
    {
        // Si hay spawn points definidos, usar uno aleatorio
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            return spawnPoints[randomIndex].position;
        }

        // GDD: El Ex aparece FRENTE al jugador
        if (playerTransform != null)
        {
            // Generar posición frente al jugador con variación lateral pequeña
            // Ángulo frontal: entre -30° y +30° respecto al frente
            float angleVariation = Random.Range(-30f, 30f) * Mathf.Deg2Rad;
            
            // Dirección base: hacia adelante del jugador (eje Z positivo en mundo)
            float x = Mathf.Sin(angleVariation) * spawnRadius;
            float z = Mathf.Cos(angleVariation) * spawnRadius;
            
            return new Vector3(
                playerTransform.position.x + x,
                spawnHeight,
                playerTransform.position.z + z
            );
        }

        // Fallback: posición por defecto frente
        return new Vector3(0, spawnHeight, spawnRadius);
    }

    /// <summary>
    /// Programa el siguiente spawn
    /// </summary>
    void ScheduleNextSpawn()
    {
        float variance = Random.Range(-intervalVariance, intervalVariance);
        float currentInterval = Mathf.Max(0.5f, spawnInterval - (obstaclesSpawned * difficultyRamp));
        nextSpawnTime = Time.time + currentInterval + variance;
    }

    /// <summary>
    /// Maneja el resultado de un obstáculo (esquivado o golpeado)
    /// </summary>
    void HandleObstacleResult(bool wasDodged)
    {
        OnObstacleResult?.Invoke(wasDodged);
    }

    /// <summary>
    /// Inicia el spawning de obstáculos
    /// </summary>
    public void StartSpawning()
    {
        isSpawning = true;
        nextSpawnTime = Time.time + 1f; // Pequeño delay inicial
        Debug.Log("ObstacleSpawner: Iniciando spawning");
    }

    /// <summary>
    /// Detiene el spawning
    /// </summary>
    public void StopSpawning()
    {
        isSpawning = false;
        Debug.Log("ObstacleSpawner: Spawning detenido");
    }

    /// <summary>
    /// Resetea el spawner para una nueva partida
    /// </summary>
    public void ResetSpawner()
    {
        StopSpawning();
        obstaclesSpawned = 0;
        currentSpeed = baseSpeed;

        // Desactivar todos los obstáculos
        foreach (Obstacle obs in obstaclePool)
        {
            obs.Deactivate();
        }
    }

    /// <summary>
    /// Desactiva todos los obstáculos activos
    /// </summary>
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
