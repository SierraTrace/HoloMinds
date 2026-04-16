using UnityEngine;
using UnityEngine.XR.Management;
using System.Collections;

// Controla cuándo se activa el modo VR (Cardboard)
public class VRModeController : MonoBehaviour
{
    [Header("Debug")]
    public string status = "Iniciando...";
    public bool vrActivo = false;
    public bool showDebugUI = true;

    void Awake()
    {
        Debug.Log("VRModeController: Awake llamado");
    }

    void Start()
    {
        Debug.Log("VRModeController: Start llamado");
        status = "Start - iniciando corrutina...";
        StartCoroutine(StartXRCoroutine());
    }

    void OnDisable()
    {
        Debug.Log("VRModeController: OnDisable - deteniendo XR");
        StopXR();
    }

    void OnDestroy()
    {
        Debug.Log("VRModeController: OnDestroy - deteniendo XR");
        StopXR();
    }

    IEnumerator StartXRCoroutine()
    {
        status = "Esperando 1 segundo...";
        yield return new WaitForSeconds(1f);

        status = "Comprobando XRGeneralSettings...";
        Debug.Log("VRModeController: Comprobando XRGeneralSettings");

        if (XRGeneralSettings.Instance == null)
        {
            status = "ERROR: XRGeneralSettings.Instance es NULL";
            Debug.LogError("VRModeController: XRGeneralSettings.Instance es NULL");
            yield break;
        }

        status = "Comprobando Manager...";
        if (XRGeneralSettings.Instance.Manager == null)
        {
            status = "ERROR: Manager es NULL";
            Debug.LogError("VRModeController: Manager es NULL");
            yield break;
        }

        status = "Comprobando si ya está inicializado...";
        if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            status = "XR ya inicializado previamente";
            vrActivo = true;
            Debug.Log("VRModeController: XR ya estaba inicializado");
            yield break;
        }

        status = "Inicializando XR Loader...";
        Debug.Log("VRModeController: Llamando InitializeLoader()");
        
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

        status = "Comprobando activeLoader...";
        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            status = "ERROR: activeLoader es NULL";
            Debug.LogError("VRModeController: activeLoader es NULL");
            yield break;
        }

        status = "Iniciando subsistemas XR...";
        Debug.Log("VRModeController: Llamando StartSubsystems()");
        XRGeneralSettings.Instance.Manager.StartSubsystems();

        vrActivo = true;
        status = "VR ACTIVO - " + XRGeneralSettings.Instance.Manager.activeLoader.name;
        Debug.Log("VRModeController: VR activado con: " + XRGeneralSettings.Instance.Manager.activeLoader.name);
    }

    void StopXR()
    {
        if (XRGeneralSettings.Instance != null && 
            XRGeneralSettings.Instance.Manager != null &&
            XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            vrActivo = false;
            status = "VR detenido";
        }
    }

    void OnGUI()
    {
        if (!showDebugUI) return;

        int y = 50;
        GUI.skin.label.fontSize = 24;

        GUI.Label(new Rect(20, y, 800, 40), "Status: " + status); y += 40;
        GUI.Label(new Rect(20, y, 800, 40), "VR Activo: " + vrActivo); y += 40;
        GUI.Label(new Rect(20, y, 800, 40), "Plataforma: " + Application.platform); y += 40;

        if (XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null)
        {
            GUI.Label(new Rect(20, y, 800, 40), "isInitComplete: " + XRGeneralSettings.Instance.Manager.isInitializationComplete); y += 40;
            
            var loader = XRGeneralSettings.Instance.Manager.activeLoader;
            GUI.Label(new Rect(20, y, 800, 40), "Loader: " + (loader != null ? loader.name : "NULL"));
        }
    }
}
