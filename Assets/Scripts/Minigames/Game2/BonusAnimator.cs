using UnityEngine;
using TMPro;
using System.Collections;

public class BonusAnimator : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float duration = 0.8f;
    public Vector3 startScale = new Vector3(2.5f, 2.5f, 1);
    public Vector3 endScale = new Vector3(0.5f, 0.5f, 1);

    public void StartAnimation(Vector3 startPos, Transform targetTransform)
    {
        gameObject.SetActive(true);
        StartCoroutine(AnimateRoutine(startPos, targetTransform));
    }

    private IEnumerator AnimateRoutine(Vector3 startPos, Transform target)
    {
        float elapsed = 0f;
        transform.position = startPos;
        transform.localScale = startScale;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayReguardSound();
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float curve = t * t * (3f - 2f * t);    // Curva de movimiento suave
            transform.position = Vector3.Lerp(startPos, target.position, curve);
            transform.localScale = Vector3.Lerp(startScale, endScale, curve);
            yield return null;
        }        

        gameObject.SetActive(false);
    }
}
