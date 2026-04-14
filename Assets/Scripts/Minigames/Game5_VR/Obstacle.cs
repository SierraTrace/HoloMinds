using UnityEngine;

/// <summary>
/// Comportamiento del "Ex" que se acerca al jugador.
/// Según GDD: El jugador esquiva girando la cabeza a izquierda o derecha (yaw).
/// </summary>
public class Obstacle : MonoBehaviour
{
    [Header("Movimiento")]
    [Tooltip("Velocidad de acercamiento al jugador")]
    public float speed = 5f;
    
    [Tooltip("Posición objetivo (la cámara del jugador)")]
    public Transform target;

    [Header("Detección de Esquive (GDD)")]
    [Tooltip("Distancia a la que se evalúa si el jugador esquivó")]
    public float evaluationDistance = 2.5f;
    
    [Tooltip("Distancia mínima antes de desaparecer")]
    public float despawnDistance = 0.5f;
    
    [Tooltip("Ángulo mínimo de giro horizontal para considerar esquiva (grados)")]
    public float dodgeAngleThreshold = 25f;

    [Header("Estado")]
    [SerializeField] private bool hasBeenEvaluated = false;
    [SerializeField] private bool isActive = true;

    // Evento para comunicar con el GameManager
    public System.Action<bool> OnObstacleResult; // true = esquivado, false = abrazo

    private VRCameraController cameraController;
    private Vector3 initialApproachDirection;

    void Start()
    {
        if (target == null)
        {
            target = Camera.main?.transform;
        }
        cameraController = FindFirstObjectByType<VRCameraController>();
        
        // Guardar dirección inicial de aproximación
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

    void MoveTowardsTarget()
    {
        if (target == null) return;

        // Moverse directamente hacia el target
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        
        // Mirar al jugador
        transform.LookAt(target);
    }

    void CheckEvaluation()
    {
        if (target == null || hasBeenEvaluated) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // ¿Llegó a la zona de evaluación?
        if (distanceToTarget <= evaluationDistance)
        {
            EvaluateDodge();
        }

        // ¿Llegó demasiado cerca? Desactivar
        if (distanceToTarget <= despawnDistance)
        {
            Deactivate();
        }
    }

    /// <summary>
    /// Evalúa si el jugador esquivó girando la cabeza a los lados (GDD)
    /// </summary>
    void EvaluateDodge()
    {
        if (hasBeenEvaluated) return;
        hasBeenEvaluated = true;

        bool isDodged = IsPlayerLookingAway();

        OnObstacleResult?.Invoke(isDodged);

        if (isDodged)
        {
            Debug.Log("Obstacle: ¡Esquivaste al Ex! (+autoestima)");
            ShowDodgeEffect();
        }
        else
        {
            Debug.Log("Obstacle: ¡Tu Ex te abrazó! (-autoestima)");
            ShowHitEffect();
        }
    }

    /// <summary>
    /// Comprueba si el jugador está mirando hacia un lado (esquiva por giro horizontal/yaw)
    /// Según GDD: El jugador debe girar la cabeza a izquierda o derecha para esquivar
    /// </summary>
    bool IsPlayerLookingAway()
    {
        Transform cameraTransform = cameraController != null ? cameraController.transform : Camera.main?.transform;
        
        if (cameraTransform == null) return false;

        // Obtener la dirección hacia donde mira la cámara (solo componente horizontal)
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0; // Ignorar componente vertical
        cameraForward.Normalize();

        // Obtener la dirección desde la cámara hacia el obstáculo (solo horizontal)
        Vector3 toObstacle = (transform.position - cameraTransform.position);
        toObstacle.y = 0; // Ignorar componente vertical
        toObstacle.Normalize();

        // Calcular el ángulo horizontal entre donde mira el jugador y donde está el Ex
        float horizontalAngle = Vector3.Angle(cameraForward, toObstacle);

        // Si el ángulo es mayor que el umbral, el jugador está mirando hacia otro lado = ESQUIVA
        bool isDodging = horizontalAngle > dodgeAngleThreshold;
        
        Debug.Log($"Obstacle: Ángulo horizontal: {horizontalAngle:F1}° (umbral: {dodgeAngleThreshold}°) -> {(isDodging ? "ESQUIVA" : "ABRAZO")}");
        
        return isDodging;
    }

    void ShowDodgeEffect()
    {
        // Efecto visual de esquive - color verde
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.green;
        }
        // TODO: Añadir partículas, sonido positivo
    }

    void ShowHitEffect()
    {
        // Efecto visual de abrazo/impacto - color rojo
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.red;
        }
        // TODO: Añadir efecto de abrazo, sonido negativo, shake de cámara
    }

    public void Deactivate()
    {
        isActive = false;
        gameObject.SetActive(false);
    }

    public void ResetObstacle(Vector3 spawnPosition, float newSpeed)
    {
        transform.position = spawnPosition;
        speed = newSpeed;
        hasBeenEvaluated = false;
        isActive = true;
        
        // Resetear color
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.white;
        }
        
        // Recalcular dirección de aproximación
        if (target != null)
        {
            initialApproachDirection = (target.position - transform.position).normalized;
        }
        
        gameObject.SetActive(true);
    }
}
