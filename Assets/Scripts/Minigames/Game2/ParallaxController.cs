using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    [System.Serializable]
    public struct ParaxLayer
    {
        public Renderer renderer;
        public float speed;         //Velocidad 0 para el cielo, alta para la calle, media para los edificios
    }

    public ParaxLayer[] layers;
    public float globalSpeed = 1f;

    private float distanceMoved = 0f;
    public static float currentWorldSpeed;
    public static float roadSpeedReference;         // Velocidad de la calle para otros scripts


    private void Update()
    {
        currentWorldSpeed = globalSpeed;
        distanceMoved += Time.deltaTime * globalSpeed;

        foreach (var layer in layers)
        {
            float offset = distanceMoved * layer.speed;
            layer.renderer.material.mainTextureOffset = new Vector2(offset, 0);

            if (layer.renderer.name.Contains("Layer5_Road"))
            {
                roadSpeedReference = layer.speed;
            }
        }
    }
}
