using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public GameObject pensamientoPrefab;
    public Sprite iconoEx;
    public List<Sprite> iconosNeutros;

    private float intervaloActual = 1.0f;
    private float proximoSpawnTime;

    void Update()
    {
        if (GameManagerAutoestima.instance == null || !GameManagerAutoestima.instance.juegoActivo) return;

        float tiempoTranscurrido = 10f - GameManagerAutoestima.instance.tiempoRestante;

        // Curva de dificultad
        if (tiempoTranscurrido < 3f) intervaloActual = 0.8f;
        else if (tiempoTranscurrido < 6f) intervaloActual = 0.4f;
        else intervaloActual = 0.2f;

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

        // 40% corazones para que sea posible llegar a la meta de 50 puntos
        bool esEx = Random.value < 0.4f;

        Sprite sp = esEx ? iconoEx : iconosNeutros[Random.Range(0, iconosNeutros.Count)];

        float multiplicadorVel = 1 + (Mathf.Floor((10f - GameManagerAutoestima.instance.tiempoRestante) / 3f) * 0.3f);

        p.GetComponent<Pensamiento>().Configurar(dir, esEx, sp, 4.5f * multiplicadorVel);
    }
}