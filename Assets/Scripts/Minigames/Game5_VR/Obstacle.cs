using UnityEngine;

// Comportamiento del "Ex" que se acerca al jugador
// El jugador esquiva girando la cabeza a izquierda o derecha (yaw)
public class Obstacle : MonoBehaviour
{
    [Header("Movimiento")]
    [Tooltip("Velocidad de acercamiento al jugador")]
    public float speed = 5f;
    
    [Tooltip("Posición objetivo (la cámara del jugador)")]
    public Transform target;

    [Header("Detección de Esquive (GDD)")]
    [Tooltip("Distancia a la que se evalúa si el jugador esquiva")]
    public float evaluationDistance = 2.5f;
    
    [Tooltip("Distancia mínima antes de desaparecer")]
    public float despawnDistance = 0.5f;
    
    [Tooltip("Ángulo mínimo de giro horizontal para considerar esquiva (grados)")]
    public float dodgeAngleThreshold = 25f;

    [Header("Estado")]
    [SerializeField] private bool hasBeenEvaluated = false;
    [SerializeField] private bool isActive = true;

    // Evento para comunicar con el GameManager
    public System.Action<bool> OnObstacleResult;

    private VRCameraController cameraController;
    private Vector3 initialApproachDirection;

    void Start()
    {
        if (target == null)
        {
            target = Camera.main?.transform;
        }
        cameraController = FindFirstObjectByType<VRCameraController>();
        
        if (target != null)
        {
            initialApproachDirection = (target.position - transform.position).normalized;
        }
    }

    void Update()
    {
        if (!isActive) return;
        MoveTowardsTarget();
        CheckEvaluation();
    }

    // Mueve el obstáculo hacia el jugador
    void MoveTowardsTarget()
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        transform.LookAt(target);
    }

    // Comprueba si el obstáculo llega a la zona de evaluación
    void CheckEvaluation()
    {
        if (target == null || hasBeenEvaluated) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= evaluationDistance)
        {
            EvaluateDodge();
        }

        if (distanceToTarget <= despawnDistance)
        {
            Deactivate();
        }
    }

    // Evalúa si el jugador esquiva girando la cabeza a los lados
    void EvaluateDodge()
    {
        if (hasBeenEvaluated) return;
        hasBeenEvaluated = true;

        bool isDodged = IsPlayerLookingAway();

        OnObstacleResult?.Invoke(isDodged);

        if (isDodged)
        {
            Debug.Log("Obstacle: ¡Esquivas al Ex! (+autoestima)");
            ShowDodgeEffect();
        }
        else
        {
            Debug.Log("Obstacle: ¡Tu Ex te abraza! (-autoestima)");
            ShowHitEffect();
        }
    }

    // Comprueba si el jugador mira hacia otro lado (esquiva por giro horizontal/yaw)
    bool IsPlayerLookingAway()
    {
        Transform cameraTransform = cameraController != null ? cameraController.transform : Camera.main?.transform;
        
        if (cameraTransform == null) return false;

        // Obtiene la dirección hacia donde mira la cámara (solo horizontal)
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        // Obtiene la dirección desde la cámara hacia el obstáculo (solo horizontal)
        Vector3 toObstacle = (transform.position - cameraTransform.position);
        toObstacle.y = 0;
        toObstacle.Normalize();

        // Calcula el ángulo horizontal entre donde mira el jugador y donde está el Ex
        float horizontalAngle = Vector3.Angle(cameraForward, toObstacle);

        // Si el ángulo es mayor que el umbral, el jugador mira hacia otro lado = ESQUIVA
        bool isDodging = horizontalAngle > dodgeAngleThreshold;
        
        Debug.Log($"Obstacle: Ángulo horizontal: {horizontalAngle:F1}° (umbral: {dodgeAngleThreshold}°) -> {(isDodging ? "ESQUIVA" : "ABRAZO")}");
        
        return isDodging;
    }

    // Efecto visual de esquive - color verde
    void ShowDodgeEffect()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.green;
        }
    }

    // Efecto visual de abrazo/impacto - color rojo
    void ShowHitEffect()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.red;
        }
    }

    // Desactiva el obstáculo (para object pooling)
    public void Deactivate()
    {
        isActive = false;
        gameObject.SetActive(false);
    }

    // Resetea el obstáculo para reutilizarlo
    public void ResetObstacle(Vector3 spawnPosition, float newSpeed)
    {
        transform.position = spawnPosition;
        speed = newSpeed;
        hasBeenEvaluated = false;
        isActive = true;
        
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.white;
        }
        
        if (target != null)
        {
            initialApproachDirection = (target.position - transform.position).normalized;
        }
        
        gameObject.SetActive(true);
    }
}
