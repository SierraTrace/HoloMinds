using UnityEngine;

public class ObjetoAR : MonoBehaviour
{
    public bool debeElimibarse;
    public int valorPuntos = 10;
    public float tiempoVida = 3f;

    void Start()
    {
        Destroy(gameObject, tiempoVida);
    }
}
