using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SpawnerAR : MonoBehaviour
{
    [Header("Configuración AR")]
    public ARPlaneManager planeManager; 
    public float tiempoEntreSpawns = 2f; 

    [Header("Clasificación de Objetos")]
    // Ahora tenemos DOS listas separadas
    public GameObject[] objetosParaSuelo; // Zapatos, basura, cajas...
    public GameObject[] objetosParaMesa;  // Marcos de fotos, tazas, mandos...

    [Header("Ajuste de Altura")]
    [Tooltip("Cualquier plano por encima de este valor será considerado 'Mesa/Sofá'")]
    public float alturaLimiteSuelo = -1.0f; 

    private float tiempoAcumulado = 0f;

    void Update()
    {
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

        // Elegimos un plano al azar como hacíamos antes
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

        if (planoElegido != null)
        {
            // Calculamos la posición dentro del plano
            Vector3 centroPlano = planoElegido.transform.position;
            float xAleatorio = Random.Range(-planoElegido.extents.x, planoElegido.extents.x);
            float zAleatorio = Random.Range(-planoElegido.extents.y, planoElegido.extents.y); 
            Vector3 posicionSpawn = centroPlano + (planoElegido.transform.rotation * new Vector3(xAleatorio, 0, zAleatorio));

            
            // Comprobamos la altura (Eje Y) del plano elegido
            GameObject prefabElegido = null;

            // En AR, la cámara suele empezar en Y = 0. 
            // El suelo real suele estar a -1.5 metros aprox. Una mesa a -0.5 metros.
            if (centroPlano.y > alturaLimiteSuelo)
            {
                // El plano está alto, asumimos que es una MESA o SOFÁ
                if (objetosParaMesa.Length > 0) {
                    prefabElegido = objetosParaMesa[Random.Range(0, objetosParaMesa.Length)];
                }
            }
            else
            {
               
                if (objetosParaSuelo.Length > 0) {
                    prefabElegido = objetosParaSuelo[Random.Range(0, objetosParaSuelo.Length)];
                }
            }

            // Si hemos encontrado un objeto válido, lo hacemos aparecer
            if (prefabElegido != null) {
                Instantiate(prefabElegido, posicionSpawn, Quaternion.identity);
            }
        }
    }
}