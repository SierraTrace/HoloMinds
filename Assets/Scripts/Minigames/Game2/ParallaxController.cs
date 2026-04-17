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

    private void Update()
    {
        foreach (var layer in layers)
        {
            float offset = Time.time * layer.speed * globalSpeed;
            layer.renderer.material.mainTextureOffset = new Vector2(offset, 0);
        }
    }
}
