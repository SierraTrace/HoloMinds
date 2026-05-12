using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SpawnerAR : MonoBehaviour
{
    [Header("Configuración AR")]
    public ARPlaneManager planeManager;

    [Header("Clasificación de Objetos")]
    public GameObject[] objetosParaSuelo;
    public GameObject[] objetosParaMesa;

    [Header("Fallback de Altura")]
    [Tooltip("Se usa solo si AR Foundation no puede clasificar el plano automáticamente")]
    public float alturaLimiteSuelo = -1.0f;

    [Header("Dificultad Progresiva")]
    [Tooltip("Segundos entre spawns al inicio")]
    public float tiempoEntreSpawnsInicial = 1.5f;
    [Tooltip("Segundos entre spawns en el pico de dificultad")]
    public float tiempoEntreSpawnsMinimo = 0.4f;
    [Tooltip("Segundos de juego hasta alcanzar la dificultad máxima")]
    public float tiempoHastaMaxDificultad = 60f;

    private float tiempoAcumulado = 0f;
    private float tiempoJuego = 0f;
    private float tiempoEntreSpawns;

    void Start()
    {
        tiempoEntreSpawns = tiempoEntreSpawnsInicial;
    }

    void Update()
    {
        tiempoJuego += Time.deltaTime;
        float t = Mathf.Clamp01(tiempoJuego / tiempoHastaMaxDificultad);
        tiempoEntreSpawns = Mathf.Lerp(tiempoEntreSpawnsInicial, tiempoEntreSpawnsMinimo, t);

        tiempoAcumulado += Time.deltaTime;
        if (tiempoAcumulado >= tiempoEntreSpawns)
        {
            SpawnearObjeto();
            tiempoAcumulado = 0f;
        }
    }

    void SpawnearObjeto()
    {
        var planos = planeManager.trackables;
        if (planos.count == 0) return;

        int indiceAleatorio = Random.Range(0, planos.count);
        int i = 0;
        ARPlane planoElegido = null;

        foreach (var plano in planos)
        {
            if (i == indiceAleatorio)
            {
                planoElegido = plano;
                break;
            }
            i++;
        }

        if (planoElegido == null) return;

        Vector3 centroPlano = planoElegido.transform.position;
        float xAleatorio = Random.Range(-planoElegido.extents.x, planoElegido.extents.x);
        float zAleatorio = Random.Range(-planoElegido.extents.y, planoElegido.extents.y);
        Vector3 posicionSpawn = centroPlano + (planoElegido.transform.rotation * new Vector3(xAleatorio, 0, zAleatorio));

        bool esMesa;
        var clasificacion = planoElegido.classification;
        if (clasificacion == PlaneClassification.Table || clasificacion == PlaneClassification.Seat)
            esMesa = true;
        else if (clasificacion == PlaneClassification.Floor)
            esMesa = false;
        else
            esMesa = centroPlano.y > alturaLimiteSuelo;

        GameObject prefabElegido = null;
        if (esMesa)
        {
            if (objetosParaMesa.Length > 0)
                prefabElegido = objetosParaMesa[Random.Range(0, objetosParaMesa.Length)];
        }
        else
        {
            if (objetosParaSuelo.Length > 0)
                prefabElegido = objetosParaSuelo[Random.Range(0, objetosParaSuelo.Length)];
        }

        if (prefabElegido == null) return;

        float radioSeguridad = 0.2f;
        bool posicionLibre = true;
        Collider[] cercanos = Physics.OverlapSphere(posicionSpawn, radioSeguridad);
        foreach (var col in cercanos)
        {
            if (col.GetComponent<ObjetoAR>() != null)
            {
                posicionLibre = false;
                break;
            }
        }

        if (posicionLibre)
        {
            Quaternion rotacionBuena = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            Instantiate(prefabElegido, posicionSpawn, rotacionBuena);
        }
    }
}
