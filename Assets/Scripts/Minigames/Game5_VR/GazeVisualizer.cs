using UnityEngine;

/// <summary>
/// Visualiza hacia dónde está mirando el jugador (debug y feedback visual).
/// Dibuja un raycast y puede mostrar un reticle/punto de mira.
/// </summary>
public class GazeVisualizer : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Mostrar línea de debug en el Editor")]
    public bool showDebugRay = true;
    
    [Tooltip("Longitud del raycast visual")]
    public float rayLength = 50f;
    
    [Tooltip("Color del rayo cuando no apunta a nada")]
    public Color normalColor = Color.white;
    
    [Tooltip("Color del rayo cuando apunta a un obstáculo")]
    public Color targetingColor = Color.red;

    [Header("Reticle (Punto de mira)")]
    [Tooltip("Usar un reticle visual")]
    public bool useReticle = true;
    
    [Tooltip("GameObject del reticle (debe ser hijo de la cámara)")]
    public GameObject reticlePrefab;
    
    [Tooltip("Distancia del reticle desde la cámara")]
    public float reticleDistance = 5f;

    [Header("Estado")]
    [SerializeField] private bool isTargetingObstacle = false;
    [SerializeField] private Obstacle currentTarget;

    private GameObject reticleInstance;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        
        if (useReticle && reticlePrefab != null)
        {
            CreateReticle();
        }
    }

    void Update()
    {
        UpdateGazeDetection();
        UpdateReticle();
        
        if (showDebugRay)
        {
            DrawDebugRay();
        }
    }

    /// <summary>
    /// Crea el reticle como hijo de la cámara
    /// </summary>
    void CreateReticle()
    {
        reticleInstance = Instantiate(reticlePrefab, mainCamera.transform);
        reticleInstance.transform.localPosition = new Vector3(0, 0, reticleDistance);
        reticleInstance.transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// Detecta si el gaze está apuntando a un obstáculo
    /// </summary>
    void UpdateGazeDetection()
    {
        if (mainCamera == null) return;

        Ray gazeRay = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(gazeRay, out hit, rayLength))
        {
            Obstacle obstacle = hit.collider.GetComponent<Obstacle>();
            
            if (obstacle != null)
            {
                isTargetingObstacle = true;
                currentTarget = obstacle;
            }
            else
            {
                isTargetingObstacle = false;
                currentTarget = null;
            }
        }
        else
        {
            isTargetingObstacle = false;
            currentTarget = null;
        }
    }

    /// <summary>
    /// Actualiza la posición y apariencia del reticle
    /// </summary>
    void UpdateReticle()
    {
        if (reticleInstance == null || mainCamera == null) return;

        // Posicionar el reticle
        Ray gazeRay = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(gazeRay, out hit, rayLength))
        {
            // Colocar reticle en el punto de impacto
            reticleInstance.transform.position = hit.point - mainCamera.transform.forward * 0.1f;
            
            // Escalar según distancia (más pequeño cuando está cerca)
            float scale = Mathf.Clamp(hit.distance / reticleDistance, 0.5f, 2f);
            reticleInstance.transform.localScale = Vector3.one * scale * 0.1f;
        }
        else
        {
            // Colocar reticle a distancia fija
            reticleInstance.transform.localPosition = new Vector3(0, 0, reticleDistance);
            reticleInstance.transform.localScale = Vector3.one * 0.1f;
        }

        // Cambiar color según si apunta a un obstáculo
        Renderer rend = reticleInstance.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = isTargetingObstacle ? targetingColor : normalColor;
        }
    }

    /// <summary>
    /// Dibuja el rayo de debug en el Editor
    /// </summary>
    void DrawDebugRay()
    {
        if (mainCamera == null) return;

        Color rayColor = isTargetingObstacle ? targetingColor : normalColor;
        Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * rayLength, rayColor);
    }

    /// <summary>
    /// Obtiene el obstáculo actualmente en la mira
    /// </summary>
    public Obstacle GetCurrentTarget()
    {
        return currentTarget;
    }

    /// <summary>
    /// Comprueba si el gaze está apuntando a algún obstáculo
    /// </summary>
    public bool IsTargetingObstacle()
    {
        return isTargetingObstacle;
    }
}
