using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controlador de cámara VR que funciona tanto en Editor (simulador) como en dispositivo (giroscopio).
/// Compatible con New Input System de Unity.
/// Attach este script a la cámara principal.
/// </summary>
public class VRCameraController : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Sensibilidad de rotación en modo simulador (ratón)")]
    public float mouseSensitivity = 0.2f;
    
    [Tooltip("Usar giroscopio en dispositivo móvil")]
    public bool useGyroscope = true;

    [Header("Debug")]
    [SerializeField] private bool isGyroscopeAvailable;
    [SerializeField] private bool isUsingSimulator;

    // Usar UnityEngine.Gyroscope explícitamente para evitar ambigüedad con InputSystem.Gyroscope
    private UnityEngine.Gyroscope gyro;
    private Quaternion gyroRotation;
    
    // Para simulador en Editor
    private float rotationX = 0f;
    private float rotationY = 0f;
    
    // New Input System
    private Mouse mouse;
    private Keyboard keyboard;

    void Start()
    {
        InitializeInput();
    }

    void InitializeInput()
    {
        // Obtener referencias del New Input System
        mouse = Mouse.current;
        keyboard = Keyboard.current;
        
        // Intentar activar giroscopio en móvil (usa UnityEngine.Input.gyro)
        if (SystemInfo.supportsGyroscope && useGyroscope)
        {
            gyro = UnityEngine.Input.gyro;
            gyro.enabled = true;
            isGyroscopeAvailable = true;
            isUsingSimulator = false;
            Debug.Log("VRCameraController: Giroscopio activado");
        }
        else
        {
            isGyroscopeAvailable = false;
            isUsingSimulator = true;
            Debug.Log("VRCameraController: Modo simulador (ratón)");
        }
    }

    void Update()
    {
        // Actualizar referencias por si se conectan/desconectan dispositivos
        if (mouse == null) mouse = Mouse.current;
        if (keyboard == null) keyboard = Keyboard.current;
        
        if (isGyroscopeAvailable && useGyroscope)
        {
            UpdateGyroscope();
        }
        else
        {
            UpdateSimulator();
        }
    }

    /// <summary>
    /// Actualiza la rotación usando el giroscopio del dispositivo
    /// </summary>
    void UpdateGyroscope()
    {
        // Convertir rotación del giroscopio a rotación de Unity
        gyroRotation = GyroToUnity(gyro.attitude);
        transform.localRotation = gyroRotation;
    }

    /// <summary>
    /// Convierte la rotación del giroscopio al sistema de coordenadas de Unity
    /// </summary>
    private Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w) * Quaternion.Euler(90f, 0f, 0f);
    }

    /// <summary>
    /// Simula rotación VR con el ratón (para testing en Editor)
    /// Controles: Click derecho + mover ratón, o mantener Alt + mover ratón
    /// </summary>
    void UpdateSimulator()
    {
        if (mouse == null) return;
        
        // Comprobar si se mantiene click derecho o Alt
        bool isControlling = false;
        
        if (mouse.rightButton.isPressed)
        {
            isControlling = true;
        }
        
        if (keyboard != null && keyboard.leftAltKey.isPressed)
        {
            isControlling = true;
        }
        
        if (isControlling)
        {
            // Leer delta del ratón con New Input System
            Vector2 mouseDelta = mouse.delta.ReadValue();
            
            rotationX += mouseDelta.x * mouseSensitivity;
            rotationY -= mouseDelta.y * mouseSensitivity;
            
            // Limitar rotación vertical para que sea realista
            rotationY = Mathf.Clamp(rotationY, -80f, 80f);
            
            transform.localRotation = Quaternion.Euler(rotationY, rotationX, 0f);
        }
    }

    /// <summary>
    /// Obtiene la dirección hacia donde está mirando el jugador
    /// </summary>
    public Vector3 GetGazeDirection()
    {
        return transform.forward;
    }

    /// <summary>
    /// Obtiene la rotación actual de la cámara
    /// </summary>
    public Quaternion GetCurrentRotation()
    {
        return transform.localRotation;
    }

    /// <summary>
    /// Resetea la rotación a la posición inicial
    /// </summary>
    public void ResetRotation()
    {
        rotationX = 0f;
        rotationY = 0f;
        transform.localRotation = Quaternion.identity;
        
        if (isGyroscopeAvailable)
        {
            gyro.enabled = false;
            gyro.enabled = true;
        }
    }
}
