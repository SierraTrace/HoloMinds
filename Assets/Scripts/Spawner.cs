using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public GameObject pensamientoPrefab;
    public Sprite iconoEx;
    public List<Sprite> iconosNeutros;

    private float intervaloActual = 1.0f;
    private float proximoSpawnTime; // Movida aquí fuera (Variable de clase)

    void Update()
    {
        if (GameManager.instance == null || !GameManager.instance.juegoActivo) return;

        float tiempoTranscurrido = 10f - GameManager.instance.tiempoRestante;

        // Implementación de tu Curva de Dificultad del GDD
        if (tiempoTranscurrido < 3f) intervaloActual = 1.0f;
        else if (tiempoTranscurrido < 7f) intervaloActual = 0.5f;
        else intervaloActual = 0.2f;

        // Lógica de Spawn corregida
        if (Time.time >= proximoSpawnTime)
        {
            Spawn();
            proximoSpawnTime = Time.time + intervaloActual;
        }
    }

    void Spawn()
    {
        GameObject p = Instantiate(pensamientoPrefab, Vector3.zero, Quaternion.identity);
        
        float angulo = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 dir = new Vector3(Mathf.Cos(angulo), Mathf.Sin(angulo), 0);
        
        bool esEx = Random.value < 0.8f;
        Sprite sp = esEx ? iconoEx : iconosNeutros[Random.Range(0, iconosNeutros.Count)];

        // Velocidad según GDD: Base 3f + 20% cada 3s
        float multiplicadorVel = 1 + (Mathf.Floor((10f - GameManager.instance.tiempoRestante) / 3f) * 0.2f);

        // Pasamos 4 datos: dirección, tipo, imagen y velocidad calculada
        p.GetComponent<Pensamiento>().Configurar(dir, esEx, sp, 3f * multiplicadorVel);
    }
}