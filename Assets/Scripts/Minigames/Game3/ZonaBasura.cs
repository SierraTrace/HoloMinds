using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ZonaBasura : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        GameObject mensaje = eventData.pointerDrag;
        Swipear scriptSwipear = mensaje.GetComponent<Swipear>();

        if (scriptSwipear != null)
        {
            // 1. Marcar como que ya está en la basura
            scriptSwipear.estaBasura = true;

            // 2. Hacer que este objeto deje de bloquear el Raycast para los siguientes
            CanvasGroup cg = mensaje.GetComponent<CanvasGroup>();
            if (cg != null) cg.blocksRaycasts = false;

            // Apagar RaycastTarget de todos los hijos para asegurar que no bloquean nada
            foreach (var img in mensaje.GetComponentsInChildren<UnityEngine.UI.Image>())
                img.raycastTarget = false;
            
            foreach (var txt in mensaje.GetComponentsInChildren<TextMeshProUGUI>())
                txt.raycastTarget = false;

            // 3. Colocar en la papelera
            mensaje.transform.SetParent(this.transform);
            mensaje.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            mensaje.transform.localScale = new Vector3(0.6f, 0.6f, 1f);

            // 4. Lógica de puntos
            if (mensaje.CompareTag("MensajeRojo"))
            {
                Debug.Log("Ex enviado a la basura");
            }
        }
    }
}