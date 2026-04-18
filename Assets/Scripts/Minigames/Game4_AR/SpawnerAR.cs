using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SpawnerAR : MonoBehaviour
{
    [Header("Configuración AR")]
    public ARPlaneManager planeManager; 
    public float tiempoEntreSpawns = 2f; 

    [Header("Clasificación de Objetos")]
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

        // 1. Elegimos un plano al azar
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
            // 2. Calculamos la posición dentro del plano
            Vector3 centroPlano = planoElegido.transform.position;
            float xAleatorio = Random.Range(-planoElegido.extents.x, planoElegido.extents.x);
            float zAleatorio = Random.Range(-planoElegido.extents.y, planoElegido.extents.y); 
            Vector3 posicionSpawn = centroPlano + (planoElegido.transform.rotation * new Vector3(xAleatorio, 0, zAleatorio));

            // 3. Comprobamos la altura y ELEGIMOS EL PREFAB PRIMERO
            GameObject prefabElegido = null;
            if (centroPlano.y > alturaLimiteSuelo)
            {
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

            // 4. Si hemos elegido un objeto, comprobamos si cabe y lo creamos
            if (prefabElegido != null) 
            {
                float radioSeguridad = 0.7f; // <-- Sube esto a 0.5f si tus objetos siguen tocándose
                
                // Comprobamos si el radar está libre
                if (!Physics.CheckSphere(posicionSpawn, radioSeguridad))
                {
                    // Creamos una rotación que siempre mire hacia arriba, pero girada al azar en Y
                    Quaternion rotacionBuena = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

                    
                    Instantiate(prefabElegido, posicionSpawn, rotacionBuena);
                }
                else
                {
                    Debug.Log("Posición ocupada. Abortando misión."); 
                }
            }
        }
    }
}