using UnityEngine;
using UnityEngine.InputSystem;

// Controlador de cámara SOLO para testing en Editor
// En dispositivo móvil con Cardboard, este script se desactiva automáticamente
public class VRCameraController : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Sensibilidad de rotación en modo simulador (ratón)")]
    public float mouseSensitivity = 0.2f;

    private float rotationX = 0f;
    private float rotationY = 0f;
    
    private Mouse mouse;
    private Keyboard keyboard;

    void Start()
    {
        // Solo activo en Editor, en móvil Cardboard controla la cámara
        #if !UNITY_EDITOR
        enabled = false;
        return;
        #endif
        
        mouse = Mouse.current;
        keyboard = Keyboard.current;
        Debug.Log("VRCameraController: Modo simulador activo (solo Editor)");
    }

    void Update()
    {
        #if UNITY_EDITOR
        if (mouse == null) mouse = Mouse.current;
        if (keyboard == null) keyboard = Keyboard.current;
        
        UpdateSimulator();
        #endif
    }

    void UpdateSimulator()
    {
        if (mouse == null) return;
        
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
            Vector2 mouseDelta = mouse.delta.ReadValue();
            
            rotationX += mouseDelta.x * mouseSensitivity;
            rotationY -= mouseDelta.y * mouseSensitivity;
            rotationY = Mathf.Clamp(rotationY, -80f, 80f);
            
            transform.localRotation = Quaternion.Euler(rotationY, rotationX, 0f);
        }
    }
}
